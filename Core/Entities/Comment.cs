using System.Text.Json.Serialization;

namespace Core.Entities;

public class Comment : BaseEntity
{
    /// <summary>
    /// The content of the comment.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Whether the current user is the author of the comment
    /// </summary>
    public bool IsLikedByAuthor { get; set; }

    /// <summary>
    /// Number of likes this comment has gotten
    /// </summary>
    public int LikeCount { get; set; }
    
    /// <summary>
    /// Foreign key referencing the VoyagerUser who created the comment.
    /// </summary>
    public Guid VoyagerUserId { get; set; }

    /// <summary>
    /// Foreign key referencing the Voyage associated with the comment.
    /// </summary>
    public Guid VoyageId { get; set; }

    /// <summary>
    /// Navigation property to the associated Voyage.
    /// </summary>
    [JsonIgnore] 
    public Voyage Voyage { get; set; }

    /// <summary>
    /// Navigation property to the associated VoyagerUser.
    /// </summary>
    [JsonIgnore] 
    public VoyagerUser VoyagerUser { get; set; }

    /// <summary>
    /// Navigation property for the likes associated with this comment.
    /// </summary>
    [JsonIgnore]
    public ICollection<Like> Likes { get; set; } = [];
}
