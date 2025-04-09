using System.Text.Json.Serialization;
using Core.Dtos;
using Core.Enums;
using Core.Models;

namespace Core.Entities;

/// <summary>
/// Represents a voyage entity with various attributes such as title, description, 
/// location, and associated user. This entity keeps track of related stops, likes, 
/// and comments, and includes metadata like completion status and archival status.
/// </summary>
public class Voyage : UpdatableBaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string LocationName { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public int StopCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    
    public Currency Currency { get; set; }
    public int ExpectedPrice { get; set; }
    public int ActualPrice { get; set; }
    
    public string ThumbnailUrl { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = [];
    
    public bool IsArchived { get; set; }
    public bool IsCompleted { get; set; }
    
    public Guid VoyagerUserId { get; set; }
    
    public ICollection<Stop?> Stops { get; set; }
    public ICollection<Like?> Likes { get; set; }
    public IEnumerable<Comment?> Comments { get; set; }
    [JsonIgnore]
    public VoyagerUser User { get; set; }
}

public static class VoyageExtensions
{
    // Convert Voyage entity to VoyageDto
    public static VoyageDto ToDto(this Voyage voyage)
    {
        return new VoyageDto
        {
            Id = voyage.Id,
            Title = voyage.Title ?? string.Empty,
            Description = voyage.Description ?? string.Empty,
            LocationName = voyage.LocationName ?? string.Empty,
            StartDate = voyage.StartDate,
            EndDate = voyage.EndDate ?? null,
            IsCompleted = voyage.IsCompleted,
            LikeCount = voyage.LikeCount,
            StopCount = voyage.StopCount,
            CommentCount = voyage.CommentCount,
            Currency = voyage.Currency,
            ExpectedPrice = voyage.ExpectedPrice,
            ActualPrice = voyage.ActualPrice,
            ThumbnailUrl = voyage.ThumbnailUrl ?? string.Empty,
            ImageUrls = voyage.ImageUrls != null 
                ? new List<string>(voyage.ImageUrls) 
                : new List<string>(),
            Stops = voyage.Stops != null 
                ? voyage.Stops.Where(stop => stop != null)
                    .Select(stop => stop!.ToDto())
                    .ToList()
                : new List<StopDto?>(),
            Comments = voyage.Comments != null 
                ? voyage.Comments.Where(comment => comment != null)
                    .Select(comment => comment!.ToDto())
                    .ToList()
                : new List<CommentDto?>(),
            CreatedAt = voyage.CreatedAt,
            UpdatedAt = voyage.UpdatedAt,
            VoyagerUserId = voyage.VoyagerUserId
        };
    }

    // Convert CreateVoyageModel to Voyage entity
    public static Voyage ToEntity(this CreateVoyageModel model)
    {
        return new Voyage
        {
            Title = model.Title,
            Description = model.Description,
            LocationName = model.LocationName,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            IsCompleted = model.IsCompleted,
            ExpectedPrice = model.ExpectedPrice,
            ActualPrice = model.ActualPrice,
            Currency = model.Currency,
        };
    }

    // Convert VoyageDto back to Voyage entity (if needed)
    public static Voyage ToEntity(this VoyageDto dto)
    {
        return new Voyage
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            LocationName = dto.LocationName,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            StopCount = dto.StopCount,
            LikeCount = dto.LikeCount,
            CommentCount = dto.CommentCount,
            Currency = dto.Currency,
            ExpectedPrice = dto.ExpectedPrice,
            ActualPrice = dto.ActualPrice,
            ThumbnailUrl = dto.ThumbnailUrl,
            ImageUrls = dto.ImageUrls ?? [],
            IsCompleted = dto.IsCompleted,
            VoyagerUserId = dto.VoyagerUserId,
            Stops = dto.Stops?.Select(s => s.ToEntity()).ToList(),
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
    public static void UpdateFromModel(this Voyage voyage, UpdateVoyageModel model)
    {
        voyage.Title = model.Title ?? voyage.Title;
        voyage.Description = model.Description ?? voyage.Description;
        voyage.LocationName = model.LocationName ?? voyage.LocationName;
        voyage.StartDate = model.StartDate ?? voyage.StartDate;
        voyage.EndDate = model.EndDate ?? voyage.EndDate;
        voyage.IsCompleted = model.IsCompleted ?? voyage.IsCompleted;
        voyage.StopCount = model.StopCount ?? voyage.StopCount;
        voyage.LikeCount = model.LikeCount ?? voyage.LikeCount;
        voyage.CommentCount = model.CommentCount ?? voyage.CommentCount;
        voyage.Currency = model.Currency ?? voyage.Currency;
        voyage.ExpectedPrice = model.ExpectedPrice ?? voyage.ExpectedPrice;
        voyage.ActualPrice = model.ActualPrice ?? voyage.ActualPrice;
        voyage.ThumbnailUrl = model.ThumbnailUrl ?? voyage.ThumbnailUrl;
        voyage.ImageUrls = model.ImageUrls ?? voyage.ImageUrls;
        voyage.Stops = model.Stops ?? voyage.Stops;
        voyage.IsArchived = model.IsArchived ?? voyage.IsArchived;
    }

    
    public static Voyage CopyWith(
        this Voyage voyage, 
        Guid? id = null,
        string? title = null, 
        string? description = null,
        string? locationName = null,
        DateTime? startDate = null, 
        DateTime? endDate = null,
        int? stopCount = null,
        Currency? currency = null,
        int? actualPrice = null,
        string? thumbnailUrl = null,
        int? expectedPrice = null,
        int? likeCount = null,
        int? commentCount = null,
        bool? isArchived = null,
        bool? isCompleted = null,
        List<string>? imageUrls = null,
        Guid? voyagerUserId = null,
        string? voyagerUsername = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null,
        List<Stop?>? stops = null)
    {
        return new Voyage
        {
            Id = id ?? voyage.Id, // keep existing Id
            Title = title ?? voyage.Title,
            Description = description ?? voyage.Description,
            LocationName = locationName ?? voyage.LocationName,
            StartDate = startDate ?? voyage.StartDate,
            EndDate = endDate ?? voyage.EndDate,
            StopCount = stopCount ?? voyage.StopCount,
            Currency = currency ?? voyage.Currency,
            LikeCount = likeCount ?? voyage.LikeCount,
            CommentCount = commentCount ?? voyage.CommentCount,
            IsArchived = isArchived ?? voyage.IsArchived,
            IsCompleted = isCompleted ?? voyage.IsCompleted,
            ExpectedPrice = expectedPrice ?? voyage.ExpectedPrice,
            ActualPrice = actualPrice ?? voyage.ActualPrice,
            ThumbnailUrl = thumbnailUrl ?? voyage.ThumbnailUrl,
            VoyagerUserId = voyagerUserId ?? voyage.VoyagerUserId,
            CreatedAt = createdAt ?? voyage.CreatedAt,
            UpdatedAt = updatedAt ?? voyage.UpdatedAt,
            ImageUrls = imageUrls ?? voyage.ImageUrls,
            Stops = stops ?? voyage.Stops
        };
    }
    
    public static void SortStops(this Voyage voyage)
    {
        if (voyage.Stops != null)
        {
            voyage.Stops = voyage.Stops
                .Where(stop => stop != null)
                .OrderBy(stop => stop!.OrderIndex)
                .ToList();
        }
    }
}
