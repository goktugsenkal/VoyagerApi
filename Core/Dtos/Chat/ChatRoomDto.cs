namespace Core.Dtos.Chat;

public class ChatRoomDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsGroup { get; set; }
    public MessageDto? LastMessage { get; set; }
    public List<Guid> ParticipantIds { get; set; } = [];
    
    public string ImageUrl { get; set; } = "default/image.jpg";
    public string? BannerUrl { get; set; }
}