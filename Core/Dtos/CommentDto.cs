namespace Core.Dtos;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public bool IsLikedByAuthor { get; set; }
    public Guid VoyagerUserId { get; set; }
    public Guid VoyageId { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}