using Core.Dtos.Chat;

namespace Core.Entities.Chat;

/// <summary>
/// Ephemeral message stored only until acknowledged by all recipients.
/// </summary>
public class Message : UpdatableBaseEntity
{
    // BaseEntity provides Id (GUID) and CreatedAt
    
    /// <summary>
    /// Client-generated UUID for deduplication and sync.
    /// </summary>
    public Guid ClientMessageId { get; set; }

    public Guid ChatRoomId { get; set; }
    public ChatRoom ChatRoom { get; set; }

    public Guid SenderId { get; set; }
    public ChatUser Sender { get; set; }

    /// <summary>
    /// Only textual content is supported; remove format/enums if not needed
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property for MessageReadReceipt
    /// </summary>
    public ICollection<ChatMessageReadReceipt> MessageReadReceipts { get; set; }
    
    /// <summary>
    /// Navigation property for MessageDeliveredReceipt
    /// </summary>
    public ICollection<ChatMessageDeliveredReceipt> MessageDeliveredReceipts { get; set; }

    /// <summary>
    /// Optional reference to a voyage if the message mentions one
    /// </summary>
    public Guid? VoyageId { get; set; }
}

public static class MessageExtensions
{
    public static MessageDto ToDto(this Message message)
    {
        return new MessageDto
        {
            ServerId = message.Id,
            TimeStamp = message.CreatedAt,
            UserId = message.SenderId,
            Text = message.Text,
            ContainsVoyage = message.VoyageId.HasValue
        };
    }
}