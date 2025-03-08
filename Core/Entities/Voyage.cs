using System.Text.Json.Serialization;
using Core.Enums;

namespace Core.Entities;

/// <summary>
/// Represents a voyage entity with various attributes such as title, description, 
/// location, and associated user. This entity keeps track of related stops, likes, 
/// and comments, and includes metadata like completion status and archival status.
/// </summary>
public class Voyage : BaseEntity
{
    /// <summary>
    /// Indicates whether the voyage is completed or being planned.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// The title of the voyage.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// A detailed description of the voyage.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The name of the location associated with the voyage.
    /// </summary>
    public string LocationName { get; set; }

    /// <summary>
    /// The start date of the voyage.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// The end date of the voyage.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// The number of stops in the voyage.
    /// </summary>
    public int StopCount { get; set; }

    /// <summary>
    /// The number of likes for the voyage.
    /// </summary>
    public int LikeCount { get; set; }

    /// <summary>
    /// The number of comments on the voyage.
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// The currency used for the voyage expenses.
    /// </summary>
    public Currency Currency { get; set; }

    /// <summary>
    /// The expected price for the voyage.
    /// </summary>
    public int ExpectedPrice { get; set; }

    /// <summary>
    /// The actual price incurred for the voyage.
    /// </summary>
    public int ActualPrice { get; set; }

    /// <summary>
    /// The URL for the voyage thumbnail image.
    /// </summary>
    public string ThumbnailUrl { get; set; } = string.Empty;

    /// <summary>
    /// A list of URLs for additional images of the voyage.
    /// </summary>
    public List<string> ImageUrls { get; set; } = new List<string>();

    /// <summary>
    /// Indicates whether the voyage is archived.
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// The ID of the user associated with the voyage.
    /// </summary>
    public Guid VoyagerUserId { get; set; }

    /// <summary>
    /// Navigation property for the stops associated with the voyage.
    /// </summary>
    public ICollection<Stop?> Stops { get; set; }

    /// <summary>
    /// Navigation property for the likes associated with the voyage.
    /// </summary>
    public ICollection<Like?> Likes { get; set; }

    /// <summary>
    /// Navigation property for the comments associated with the voyage.
    /// </summary>
    public IEnumerable<Comment?> Comments { get; set; }

    /// <summary>
    /// The user associated with the voyage.
    /// </summary>
    [JsonIgnore]
    public VoyagerUser User { get; set; }
}