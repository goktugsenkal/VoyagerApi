using Core.Entities;
using Core.Enums;

namespace Core.Models;

public class UpdateVoyageModel
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? LocationName { get; set; }
    
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsCompleted { get; set; }
    public bool? IsArchived { get; set; }
    
    public int? StopCount { get; set; }
    public int? LikeCount { get; set; }
    public int? CommentCount { get; set; }

    public Currency? Currency { get; set; }
    public int? ExpectedPrice { get; set; }
    public int? ActualPrice { get; set; } = -1;
    
    public string? ThumbnailUrl { get; set; }
    public List<string>? ImageUrls { get; set; } = [];

    // voyage update model takes in create stop model
    public ICollection<Stop?>? Stops { get; set; } = [];
}