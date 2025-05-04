namespace Core.Entities.Chat;

/// <summary>
/// Acknowledgment that a user has read a specific message.
/// </summary>
public class ChatMessageReadReceipt : BaseEntity
{
    public Guid MessageId { get; set; }
    public Message Message { get; set; } = null!;

    public Guid UserId { get; set; }
    public ChatUser User { get; set; } = null!;
}