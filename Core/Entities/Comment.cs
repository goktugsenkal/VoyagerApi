using System.Text.Json.Serialization;
using Core.Dtos;

namespace Core.Entities;

public class Comment : UpdatableBaseEntity
{
    public string Content { get; set; }
    public bool IsLikedByAuthor { get; set; }
    public int LikeCount { get; set; }
    public Guid VoyagerUserId { get; set; }
    public Guid VoyageId { get; set; }
    [JsonIgnore] 
    public Voyage Voyage { get; set; }
    [JsonIgnore] 
    public VoyagerUser VoyagerUser { get; set; }
    [JsonIgnore]
    public ICollection<Like> Likes { get; set; } = [];
}

public static class CommentMappingExtensions
{
    public static CommentDto ToDto(this Comment comment)
    {
        ArgumentNullException.ThrowIfNull(comment);

        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content ?? string.Empty,
            IsLikedByAuthor = comment.IsLikedByAuthor,
            LikeCount = comment.LikeCount,
            VoyagerUserId = comment.VoyagerUserId,
            VoyageId = comment.VoyageId,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }
}