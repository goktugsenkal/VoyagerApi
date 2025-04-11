using Core.Enums;

namespace Core.Dtos;

public class VoyageDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string LocationName { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCompleted { get; set; }
    
    public int LikeCount { get; set; }
    public int StopCount { get; set; }
    public int CommentCount { get; set; }

    public Currency Currency { get; set; }
    public int ExpectedPrice { get; set; }
    public int ActualPrice { get; set; } = -1;
    
    public string ThumbnailUrl { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = [];
     
    public ICollection<StopDto?> Stops { get; set; } = [];
    public ICollection<CommentDto?> Comments { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string VoyagerUsername { get; set; } = string.Empty;
    public bool IsLiked { get; set; }
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public Guid VoyagerUserId { get; set; }
}