using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Core.Models.Voyage;

namespace Infrastructure.Services;

public class VoyageService(
    IVoyageRepository voyageRepository,
    IStopRepository stopRepository,
    IS3Service s3Service,
    IUserService userService,
    IUserRepository userRepository,
    ILikeRepository likeRepository)
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
        return Task.FromResult(new PagedList<VoyageDto>(voyageDtos, voyages.TotalCount, voyages.CurrentPage,
            voyages.PageSize));
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
    /// <param name="consumerUsedId"></param>
    /// <returns>
    /// A VoyageDto containing the voyage details if found, otherwise null.
    /// The VoyageDto includes the title, description, location, dates, like count,
    /// stop count, currency, expected and actual prices, voyager user ID, and lists
    /// of stops and comments mapped to StopDto and CommentDto respectively.
    /// </returns>
    public async Task<VoyageDto?> GetVoyageDtoByIdAsync(Guid voyageId, Guid consumerUsedId)
    {
        // try to get the entire voyage entity from the database
        var voyage = await voyageRepository.GetByIdAsync(voyageId);

        if (voyage == null) return null;

        var voyageDto = voyage.ToDto();

        var voyagerUser = await userRepository.GetByIdAsync(voyageDto.VoyagerUserId);

        voyageDto.VoyagerUsername = voyagerUser?.Username ?? "Voyager User";
        voyageDto.ProfilePictureUrl =
            s3Service.GeneratePreSignedDownloadUrl(voyagerUser?.ProfilePictureUrl ?? "", TimeSpan.FromMinutes(10));
        voyageDto.IsLiked = await likeRepository.ExistsAsync(voyageDto.Id, null, consumerUsedId);

        if (voyageDto.MediaUrls.Any())
        {
            voyageDto.MediaUrls = voyageDto.MediaUrls
                .Select(key => s3Service.GeneratePreSignedDownloadUrl(key, TimeSpan.FromMinutes(15)))
                .ToList();
        }

        if (voyageDto.Stops.Count == 0) return voyageDto;
        {
            foreach (var stopDto in voyageDto.Stops)
            {
                if (stopDto != null && stopDto.MediaUrls.Count != 0)
                {
                    stopDto.MediaUrls = stopDto.MediaUrls
                        .Select(key => s3Service.GeneratePreSignedDownloadUrl(key, TimeSpan.FromMinutes(15)))
                        .ToList();
                }
            }
        }
        return voyageDto;
    }

    public async Task<Voyage?> GetVoyageByIdAsync(Guid voyageId)
    {
        // try to get the entire voyage entity from the database
        var voyage = await voyageRepository.GetByIdAsync(voyageId);

        return voyage ?? null;
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
        var voyagerUsername = await userService.GetUsernameByIdAsync(userId)
                              ?? throw new Exception("User not found");

        var voyage = createVoyageModel.ToEntity()
            .CopyWith(
                voyagerUserId: userId,
                voyagerUsername: voyagerUsername,
                stopCount: createVoyageModel.Stops.Count
            );

        await voyageRepository.AddAsync(voyage);

        var stops = createVoyageModel.Stops.Select(stopModel =>
        {
            var entity = stopModel.ToEntity();
            entity.VoyageId = voyage.Id;
            return entity;
        }).ToList();

        await stopRepository.AddRangeAsync(stops);

        var voyageUploadUrls = new List<string>();
        var voyageMediaTypes = createVoyageModel.MediaTypes ?? new List<string>();

        foreach (var mediaType in voyageMediaTypes)
        {
            var extension = mediaType.ToLower();
            var key = $"voyages/{voyage.Id}/media/{Guid.NewGuid()}.{extension}";
            voyage.MediaKeys.Add(key);
            var url = s3Service.GeneratePreSignedUploadUrl(key, TimeSpan.FromMinutes(15));
            voyageUploadUrls.Add(url);
        }

        var stopsUploadUrls = new List<StopUploadUrlsDto>();

        for (int i = 0; i < createVoyageModel.Stops.Count; i++)
        {
            var stopRequest = createVoyageModel.Stops[i];
            var stopEntity = stops[i];
            var stopMediaTypes = stopRequest.MediaTypes ?? new List<string>();

            stopEntity.MediaKeys = new List<string>();
            var stopUrls = new List<string>();

            foreach (var mediaType in stopMediaTypes)
            {
                var extension = mediaType.ToLower();
                var key = $"voyages/{voyage.Id}/stops/{stopEntity.Id}/media/{Guid.NewGuid()}.{extension}";
                stopEntity.MediaKeys.Add(key);
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