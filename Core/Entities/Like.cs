using System.Text.Json.Serialization;
using Core.Enums;

namespace Core.Entities;

/// <summary>
/// Represents a like given by a user on a resource, such as a voyage or a comment.
/// This entity supports polymorphic relationships to either a Voyage or a Comment,
/// while maintaining the association with the user who liked the resource.
/// </summary>
public class Like : BaseEntity
{
    /// <summary>
    /// Foreign key referencing the Voyage entity. Nullable to support polymorphism.
    /// </summary>
    public Guid? VoyageId { get; set; }

    /// <summary>
    /// Foreign key referencing the Comment entity. Nullable to support polymorphism.
    /// </summary>
    public Guid? CommentId { get; set; }

    /// <summary>
    /// Indicates the type of resource being liked (Voyage or Comment).
    /// </summary>
    public LikeType LikeType { get; set; }

    /// <summary>
    /// Foreign key referencing the VoyagerUser entity.
    /// </summary>
    public Guid VoyagerUserId { get; set; }

    /// <summary>
    /// Navigation property to the associated Voyage entity.
    /// </summary>
    [JsonIgnore]
    public Voyage Voyage { get; set; }

    /// <summary>
    /// Navigation property to the associated Comment entity.
    /// </summary>
    [JsonIgnore]
    public Comment Comment { get; set; }

    /// <summary>
    /// Navigation property to the associated VoyagerUser entity.
    /// </summary>
    [JsonIgnore]
    public VoyagerUser VoyagerUser { get; set; }
}
