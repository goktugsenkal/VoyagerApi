using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services;

public class VoyageService(IVoyageRepository voyageRepository, IStopRepository stopRepository)
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

        // return null if no voyage is found
        // if found return as voyage dto 
        return voyage?.ToDto();
    }

    /// <summary>
    /// Ties together VoyageRepository and StopRepository methods for creating
    /// 1. Creates a voyage and saves it to the Voyages table
    /// 2. Gets its id
    /// 3. Maps CreateStopModel->Stop and saves them with relation to the voyage.
    /// refactored to use mapper 18.03.25
    /// </summary>
    /// <param name="createVoyageModel">CreateVoyageModel</param>
    /// <param name="voyagerUserId"></param>
    public async Task<Voyage> AddVoyageAsync(CreateVoyageModel createVoyageModel, Guid voyagerUserId)
        {
            // map request to Voyage entity
            var voyage = createVoyageModel.ToEntity(); 
            
            // assign the voyager user id
            voyage.VoyagerUserId = voyagerUserId;
                
            // this voyage will get an id in the next
            // function, "await voyageRepository.AddAsync(voyage)"

            // save the Voyage to get its id
            await voyageRepository.AddAsync(voyage);

            // map stops from CreateStopModel to Stop
            var stops = createVoyageModel.Stops.Select(stop =>
            {
                // map every CreateStopModel to Stop 
                var entity = stop.ToEntity();

                // assign the voyage id
                entity.VoyageId = voyage.Id;
                return entity;
            }).ToList();

            // save stops with relation to the Voyage
            await stopRepository.AddRangeAsync(stops);

            return voyage;
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
