using Core.Entities;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services;

public class VoyageService
    (IVoyageRepository voyageRepository, IStopRepository stopRepository)
    : IVoyageService
{
    /// <summary>
    /// Ties together VoyageRepository and StopRepository methods for creating
    /// 1. Creates a voyage and saves it to the Voyages table
    /// 2. Gets its id
    /// 3. Maps CreateStopModel->Stop and saves them with relation to the voyage.
    /// </summary>
    /// <param name="createVoyageModel">CreateVoyageModel</param>
    /// <param name="voyageUserId"></param>
    public async Task AddVoyageAsync(CreateVoyageModel createVoyageModel, Guid voyageUserId)
    {
        // map request to Voyage entity
        var voyage = new Voyage 
            // this voyage will get an id in the next
            // function, "await voyageRepository.AddAsync(voyage)"
        {
            Title = createVoyageModel.Title,
            Description = createVoyageModel.Description,
            LocationName = createVoyageModel.LocationName,
            StartDate = createVoyageModel.StartDate,
            EndDate = createVoyageModel.EndDate,
            StopCount = createVoyageModel.StopCount,
            Currency = createVoyageModel.Currency,
            ExpectedPrice = createVoyageModel.ExpectedPrice,
            ActualPrice = createVoyageModel.ActualPrice,
            VoyagerUserId = voyageUserId
        };

        // save the Voyage to get its id
        await voyageRepository.AddAsync(voyage);

        // map stops from CreateStopModel to Stop
        var stops = createVoyageModel.Stops.Select(stop => new Stop
        {
            Name = stop.Name,
            Description = stop.Description,
            Longitude = stop.Longitude,
            Latitude = stop.Latitude,
            DistanceToNext = stop.DistanceToNext,
            ArrivalTimeToNext = stop.ArrivalTimeToNext,
            TransportationTypeToNextStop = stop.TransportationTypeToNextStop,
            VoyageId = voyage.Id // set fk, voyageId
        }).ToList();
        
        // save stops with relation to the Voyage
        await stopRepository.AddRangeAsync(stops);
    }
}