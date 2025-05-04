namespace Core.Entities.Chat;

/// <summary>
/// Acknowledgment that a message has been delivered to a specific user
/// </summary>
public class ChatMessageDeliveredReceipt : BaseEntity
{
    public Guid MessageId { get; set; }
    public Message Message { get; set; } = null!;

    public Guid UserId { get; set; }
    public ChatUser User { get; set; } = null!;
}