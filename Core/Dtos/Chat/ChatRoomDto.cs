namespace Core.Dtos.Chat;

public class ChatRoomDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public MessageDto? LastMessage { get; set; }
}