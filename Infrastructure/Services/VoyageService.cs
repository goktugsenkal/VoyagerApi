using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services;

public class VoyageService(IVoyageRepository voyageRepository, IStopRepository stopRepository)
    : IVoyageService
{
    public async Task<List<VoyageDto>> GetAllVoyagesAsync()
    {
        var voyages = await voyageRepository.GetAllAsync();

        return voyages.Select(voyage => new VoyageDto
        {
            Id = voyage.Id,
            Title = voyage.Title,
            Description = voyage.Description,
            LocationName = voyage.LocationName,
            StartDate = voyage.StartDate,
            EndDate = voyage.EndDate,
            LikeCount = voyage.LikeCount,
            StopCount = voyage.StopCount,
            Currency = voyage.Currency,
            ExpectedPrice = voyage.ExpectedPrice,
            ActualPrice = voyage.ActualPrice,
            VoyagerUserId = voyage.VoyagerUserId
        }).ToList();
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
        if (voyage is null)
        {
            return null;
        }

        // map Voyage to VoyageDto
        return new VoyageDto
        {
            Id = voyage.Id,
            Title = voyage.Title,
            Description = voyage.Description,
            LocationName = voyage.LocationName,
            StartDate = voyage.StartDate,
            EndDate = voyage.EndDate,
            LikeCount = voyage.LikeCount,
            StopCount = voyage.StopCount,
            Currency = voyage.Currency,
            ExpectedPrice = voyage.ExpectedPrice,
            ActualPrice = voyage.ActualPrice,
            VoyagerUserId = voyage.VoyagerUserId,
            // map list of Stops to list of StopDtos
            Stops = voyage.Stops.Count != 0
                ? voyage.Stops
                    // if there are stops, map them to StopDtos
                    .Select(stop => new StopDto
                    {
                        Name = stop.Name,
                        Description = stop.Description,
                        Longitude = stop.Longitude,
                        Latitude = stop.Latitude,
                        DistanceToNext = stop.DistanceToNext,
                        ArrivalTimeToNext = stop.ArrivalTimeToNext,
                        TransportationTypeToNextStop = stop.TransportationTypeToNextStop,
                    })
                    .ToList()
                // if there are no stops, return an empty list
                : [],
            Comments = voyage.Comments.Any()
                ? voyage.Comments
                    .Select(comment => new CommentDto
                    {
                        Content = comment.Content
                    }).ToList()
                : []
        };
    }

    /// <summary>
        /// Ties together VoyageRepository and StopRepository methods for creating
        /// 1. Creates a voyage and saves it to the Voyages table
        /// 2. Gets its id
        /// 3. Maps CreateStopModel->Stop and saves them with relation to the voyage.
        /// </summary>
        /// <param name="createVoyageModel">CreateVoyageModel</param>
        /// <param name="voyagerUserId"></param>
        public async Task AddVoyageAsync(CreateVoyageModel createVoyageModel, Guid voyagerUserId)
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
                    VoyagerUserId = voyagerUserId
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

        public async Task UpdateVoyageAsync(Guid voyageId, UpdateVoyageModel updateVoyageModel)
        {
            var voyage = await voyageRepository.GetByIdAsync(voyageId);
            
            if (voyage is null)
            {
                throw new ArgumentException("gok: no voyage found with the specified id");
            }
            
            voyage.Title = updateVoyageModel.Title;
            voyage.Description = updateVoyageModel.Description;
            voyage.LocationName = updateVoyageModel.LocationName;
            voyage.StartDate = updateVoyageModel.StartDate;
            voyage.EndDate = updateVoyageModel.EndDate;
            voyage.StopCount = updateVoyageModel.StopCount;
            voyage.Currency = updateVoyageModel.Currency;
            voyage.ExpectedPrice = updateVoyageModel.ExpectedPrice;
            voyage.ActualPrice = updateVoyageModel.ActualPrice;
            
            await voyageRepository.UpdateAsync(voyage);
        }

        public async Task DeleteVoyageAsync(Guid voyageId)
        {
            var voyage = await voyageRepository.GetByIdAsync(voyageId);
            
            if (voyage is null)
            {
                throw new ArgumentException("gok: no voyage found with the specified id");
            }
            
            await voyageRepository.DeleteAsync(voyage);
        }
}
