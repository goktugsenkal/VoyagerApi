namespace Core.Models.Chat;

public class CreateChatRoomParticipantModel
{
    public required Guid UserId { get; set; }
    public bool IsAdmin { get; set; } = false;
}