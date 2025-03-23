using Core.Enums;

namespace Core.Dtos.FeedDtos;

public class FeedDtos
{
    /// <summary>
    /// Data Transfer Object for displaying voyages in the feed.
    /// Only the fields needed for the feed view are included.
    /// </summary>
    public class VoyageForFeedDto
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public string LocationName { get; set; } = string.Empty;
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public bool IsCompleted { get; set; }
        
        public int LikeCount { get; set; }
        
        public int StopCount { get; set; }
        
        public Currency Currency { get; set; }
        
        public int ExpectedPrice { get; set; }
        
        public int ActualPrice { get; set; }
        
        public Guid VoyagerUserId { get; set; }
        
        // Display username for the feed
        public string VoyagerUsername { get; set; } = string.Empty;
        
        // Image information for quick feed display
        public List<string> ImageUrls { get; set; } = new List<string>();
        public string ThumbnailUrl { get; set; } = string.Empty;
        
        // Creation time can be useful for sorting or display purposes
        public DateTime CreatedAt { get; set; }
        
        // Minimal data for stops in the feed
        public List<StopForFeedDto> Stops { get; set; } = new List<StopForFeedDto>();
        
        // Minimal data for comments in the feed
        public List<CommentForFeedDto> Comments { get; set; } = new List<CommentForFeedDto>();
    }

    /// <summary>
    /// Minimal DTO for displaying a stop in the feed.
    /// </summary>
    public class StopForFeedDto
    {
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public double Longitude { get; set; }
        
        public double Latitude { get; set; }
        
        // Distance and time info may be useful for quick display
        public int DistanceToNext { get; set; }
        public int ArrivalTimeToNext { get; set; }
        
        public TransportationType TransportationTypeToNextStop { get; set; }
        
        // Only the image URLs needed for previewing in the feed
        public List<string> ImageUrls { get; set; } = new List<string>();
        
        // Indicates if this stop is a focal point in the voyage
        public bool IsFocalPoint { get; set; }
    }

    /// <summary>
    /// Minimal DTO for displaying a comment in the feed.
    /// </summary>
    public class CommentForFeedDto
    {
        public string Content { get; set; } = string.Empty;
    }
}
