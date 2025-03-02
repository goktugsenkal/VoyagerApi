using Core.Dtos;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services;

public class FeedService(IVoyageRepository voyageRepository, 
        ICommentRepository commentRepository, 
        ILikeRepository likeRepository) : IFeedService
{
    public async Task<PagedList<VoyageDto>> GetFeedAsync(int pageNumber, int pageSize)
    {
        // get all voyages with paged list
        var voyages = voyageRepository.GetAllAsPagedList(pageNumber, pageSize);

        // map voyages to VoyageDtos
        var voyagesDtos = voyages.Items.Select(voyage => new VoyageDto
        {
            Id = voyage.Id,
            Title = voyage.Title,
            Description = voyage.Description,
            LocationName = voyage.LocationName,
            StartDate = voyage.StartDate,
            EndDate = voyage.EndDate,
            IsCompleted = voyage.IsCompleted,
            LikeCount = voyage.LikeCount,
            StopCount = voyage.StopCount,
            Currency = voyage.Currency,
            ExpectedPrice = voyage.ExpectedPrice,
            ActualPrice = voyage.ActualPrice,
            VoyagerUserId = voyage.VoyagerUserId,
            VoyagerUsername = voyageRepository.GetUsernameByVoyageIdAsync(voyage.Id).Result,
            ImageUrls = voyage.ImageUrls,
            ThumbnailUrl = voyage.ThumbnailUrl,
            CreatedAt = voyage.CreatedAt,
            // if there are stops
            Stops = voyage.Stops.Count != 0
                // map stops to StopDtos
                ? voyage.Stops
                    .Select(stop => new StopDto
                    {
                        Name = stop.Name,
                        Description = stop.Description,
                        Longitude = stop.Longitude,
                        Latitude = stop.Latitude,
                        DistanceToNext = stop.DistanceToNext,
                        ArrivalTimeToNext = stop.ArrivalTimeToNext,
                        TransportationTypeToNextStop = stop.TransportationTypeToNextStop,
                        ImageUrls = stop.ImageUrls ?? [],
                        IsFocalPoint = stop.IsFocalPoint
                    })
                    .ToList()
                // if there are no stops, return an empty list
                : [],
            // if there are comments
            Comments = voyage.Comments.Any()
                // map comments to CommentDtos
                ? voyage.Comments
                    .Select(comment => new CommentDto
                    {
                        // map comment content to CommentDto's content
                        Content = comment.Content
                    }).ToList()
                // if there are no comments, return an empty list
                : []
        }).ToList();
        
        // create a PagedList and return it
        return new PagedList<VoyageDto>(voyagesDtos, voyages.TotalCount, voyages.CurrentPage, voyages.PageSize);
        
    }
}