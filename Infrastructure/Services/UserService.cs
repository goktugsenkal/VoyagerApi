using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserService(IVoyageRepository voyageRepository, DataContext context) : IUserService
{
    public async Task<PagedList<VoyageDto>> GetVoyagesOfAUserAsync(string userId, int pageNumber, int pageSize)
    {
        Guid voyagerUserId;
        try
        {
            voyagerUserId = Guid.Parse(userId);
        }
        catch (Exception e)
        {
            throw new Exception("Invalid user ID format.");
        }
        
        var voyages = await voyageRepository.GetVoyagesByVoyagerUserIdAsync(voyagerUserId, pageNumber, pageSize);
        
        var voyagesDtos = voyages.Items.Select(voyage => new VoyageDto
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
            IsCompleted = voyage.IsCompleted,
            CreatedAt = voyage.CreatedAt,
            VoyagerUsername = voyageRepository.GetUsernameByVoyageIdAsync(voyage.Id).Result,
            ImageUrls = voyage.ImageUrls,
            ThumbnailUrl = voyage.ThumbnailUrl,
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
                        DistanceToNext = stop.DistanceToNextStop,
                        ArrivalTimeToNext = stop.ArrivalTimeToNextStop,
                        TransportationTypeToNextStop = stop.TransportationTypeToNextStop,
                        ImageUrls = stop.ImageUrls ?? [],
                        IsFocalPoint = stop.IsFocalPoint,
                        CreatedAt = stop.CreatedAt
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

        return new PagedList<VoyageDto>(voyagesDtos, voyages.TotalCount, voyages.CurrentPage, voyages.PageSize);
    }
    
    public async Task<VoyagerUserDto?> GetUserByIdAsync(Guid id)
    {
        //
        // todo: check if user is allowed to see the other user
        //
            
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
        {
            return null;
        }
            
        return new VoyagerUserDto
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            BannerPictureUrl = user.BannerPictureUrl,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Bio = user.Bio,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = user.CreatedAt
        };
    }
}