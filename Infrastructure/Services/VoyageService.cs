using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services;

public class VoyageService(IVoyageRepository voyageRepository, 
    IStopRepository stopRepository, 
    IS3Service s3Service, 
    IUserService userService, IUserRepository userRepository)
    : IVoyageService
{

    /// <summary>
    /// Asynchronously retrieves all voyages, paginated. Maps voyages to VoyageDtos.
    /// </summary>
    /// <param name="pageNumber">The page number of voyages to retrieve.</param>
    /// <param name="pageSize">The number of voyages per page.</param>
    /// <returns>
    /// A PagedList of VoyageDtos, containing the title, description, location name,
    /// start date, end date, like count, stop count, currency, expected price, actual price,
    /// voyager user ID, and lists of stops and comments mapped to StopDto and CommentDto
    /// respectively.
    /// </returns>
    public Task<PagedList<VoyageDto>> GetAllVoyagesAsync(int pageNumber, int pageSize)
    {
        // get all voyages with paged list
        var voyages = voyageRepository.GetAllAsPagedList(pageNumber, pageSize);

        // map voyages to VoyageDtos
        // and map stops to StopDtos
        var voyageDtos = voyages.Items.Select(voyage => voyage.ToDto()).ToList();

        // create a PagedList and return it
        return Task.FromResult(new PagedList<VoyageDto>(voyageDtos, voyages.TotalCount, voyages.CurrentPage, voyages.PageSize));

    }

    public PagedList<VoyageDto> GetVoyagesFiltered(double? latitudeMin, double? latitudeMax, double? longitudeMin,
        double? longitudeMax, int pageNumber, int pageSize)
    {
        var voyages = voyageRepository.GetVoyagesFiltered(latitudeMin, latitudeMax, longitudeMin, longitudeMax,
            pageNumber, pageSize);

        var voyageDtos = voyages.Items.Select(voyage => voyage.ToDto()).ToList();

        // create a PagedList and return it
        return new PagedList<VoyageDto>(voyageDtos, voyages.TotalCount, voyages.CurrentPage, voyages.PageSize);
    }

    public async Task<PagedList<VoyageDto>> GetVoyagesByVoyagerUserIdAsync(Guid voyagerUserId, int pageNumber,
        int pageSize)
    {
        //
        // todo: check if user is allowed to see voyages of the other user
        //

        var voyages = await voyageRepository.GetVoyagesByVoyagerUserIdAsync(voyagerUserId, pageNumber, pageSize);

        var voyagesDtos = voyages.Items.Select(voyage => voyage.ToDto()).ToList();

        // create a PagedList and return it
        return new PagedList<VoyageDto>(voyagesDtos, voyages.TotalCount, voyages.CurrentPage, voyages.PageSize);
    }

    /// <summary>
    /// Asynchronously retrieves a voyage by its ID and maps it to a VoyageDto. Maps Stops and Comments to StopDto and CommentDto.
    /// </summary>
    /// <param name="voyageId">The unique identifier of the voyage.</param>
    /// <returns>
    /// A VoyageDto containing the voyage details if found, otherwise null.
    /// The VoyageDto includes the title, description, location, dates, like count,
    /// stop count, currency, expected and actual prices, voyager user ID, and lists
    /// of stops and comments mapped to StopDto and CommentDto respectively.
    /// </returns>
    public async Task<VoyageDto?> GetVoyageByIdAsync(Guid voyageId)
    {
        // try to get the entire voyage entity from the database
        var voyage = await voyageRepository.GetByIdAsync(voyageId);
        
        if(voyage == null) return null;

        var voyageDto = voyage.ToDto();

        voyageDto.VoyagerUsername =
            await userRepository.GetUsernameByIdAsync(voyageDto.VoyagerUserId) ?? "Voyager User";

        if (voyageDto.ImageUrls.Any())
        {
            voyageDto.ImageUrls = voyageDto.ImageUrls
                .Select(key => s3Service.GeneratePreSignedDownloadUrl(key, TimeSpan.FromMinutes(15)))
                .ToList();
        }

        if (voyageDto.Stops.Count == 0) return voyageDto;
        {
            foreach (var stopDto in voyageDto.Stops)
            {
                if (stopDto != null  && stopDto.ImageUrls.Count != 0)
                {
                    stopDto.ImageUrls = stopDto.ImageUrls
                        .Select(key => s3Service.GeneratePreSignedDownloadUrl(key, TimeSpan.FromMinutes(15)))
                        .ToList();
                }
            }
        }
        return voyageDto;
    }

    /// <summary>
    /// Creates a Voyage (with its stops) and generates image keys and corresponding pre-signed URLs.
    /// </summary>
    /// <param name="createVoyageModel">The voyage creation model containing voyage and stop details including image counts.</param>
    /// <param name="userId">The user ID from the token claims.</param>
    /// <returns>A tuple containing the created Voyage, a list of pre-signed URLs for voyage images, and a list of stop upload URL DTOs.</returns>
    public async Task<(Voyage Voyage, List<string> VoyageUploadUrls, List<StopUploadUrlsDto> StopsUploadUrls)>
        AddVoyageWithMediaAsync(CreateVoyageModel createVoyageModel, Guid userId)
    {
        // get the voyager username from the user id
        var voyagerUsername = await userService.GetUsernameByIdAsync(userId) ?? throw new Exception("User not found");
        
        // Map the request to a Voyage entity and assign the voyager user id and username.
        var voyage = createVoyageModel.ToEntity()
            .CopyWith(voyagerUserId: userId, voyagerUsername: voyagerUsername, stopCount: createVoyageModel.Stops.Count);   

        // Save the voyage to get its generated ID.
        await voyageRepository.AddAsync(voyage);

        // Map stops from CreateStopModel to Stop, and assign the voyage id.
        var stops = createVoyageModel.Stops.Select(stopModel =>
        {
            var entity = stopModel.ToEntity();
            entity.VoyageId = voyage.Id;
            return entity;
        }).ToList();

        // Save stops with relation to the Voyage.
        await stopRepository.AddRangeAsync(stops);

        // Generate unique keys and pre-signed URLs for voyage images.
        var voyageUploadUrls = new List<string>();
        
        for (var i = 0; i < createVoyageModel.ImageCount; i++)
        {
            var key = $"voyages/{voyage.Id}/images/{Guid.NewGuid()}.jpg";
            voyage.ImageUrls.Add(key);
            var url = s3Service.GeneratePreSignedUploadUrl(key, TimeSpan.FromMinutes(15));
            voyageUploadUrls.Add(url);
        }

        // Generate keys and pre-signed URLs for each stop's images.
        // We assume that the order of stops in createVoyageModel.Stops corresponds to the order of persisted stops.
        var stopsUploadUrls = new List<StopUploadUrlsDto>();
        for (var i = 0; i < createVoyageModel.Stops.Count; i++)
        {
            var stopRequest = createVoyageModel.Stops[i];
            var stopEntity = stops[i]; // Using the same order for mapping.
            stopEntity.ImageUrls = new List<string>();
            var stopUrls = new List<string>();

            for (var j = 0; j < stopRequest.ImageCount; j++)
            {
                var key = $"voyages/{voyage.Id}/stops/{stopEntity.Id}/images/{Guid.NewGuid()}.jpg";
                stopEntity.ImageUrls.Add(key);
                var url = s3Service.GeneratePreSignedUploadUrl(key, TimeSpan.FromMinutes(15));
                stopUrls.Add(url);
            }

            stopsUploadUrls.Add(new StopUploadUrlsDto
            {
                StopId = stopEntity.Id,
                UploadUrls = stopUrls
            });
        }

        await voyageRepository.SaveChangesAsync();

        // Return the voyage and the generated upload URLs.
        return (voyage, voyageUploadUrls, stopsUploadUrls);
    }

        public async Task UpdateVoyageAsync(Guid voyageId, UpdateVoyageModel updateVoyageModel)
        {
            // get the voyage
            var voyage = await voyageRepository.GetByIdAsync(voyageId);
            
            // check if voyage exists
            if (voyage is null)
            {
                throw new ArgumentException("gok: no voyage found with the specified id");
            }
            
            // update the voyage with the model
            voyage.UpdateFromModel(updateVoyageModel);
            
            // save the voyage
            await voyageRepository.UpdateAsync(voyage);
        }

        public async Task DeleteVoyageAsync(Guid voyageId)
        {
            var voyage = await voyageRepository.GetByIdAsync(voyageId);
            
            if (voyage is null)
            {
                throw new ArgumentException("no voyage found with the specified id");
            }
            
            await voyageRepository.DeleteAsync(voyage);
        }
}
