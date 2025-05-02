using Core.Models.Chat;

namespace Core.Entities.Chat;

public class ChatRoomParticipant : BaseEntity
{
    public Guid ChatRoomId { get; set; }
    public ChatRoom ChatRoom { get; set; }

    public Guid UserId { get; set; }
    public ChatUser User { get; set; }

    // Only needed for group admins
    public bool IsAdmin { get; set; } = false;
}

public static class ChatRoomParticipantMappingExtensions
{
    public static ChatRoomParticipant ToEntity(this CreateChatRoomParticipantModel model, Guid chatRoomId)
    {
        return new ChatRoomParticipant
        {
            ChatRoomId = chatRoomId,
            UserId = model.UserId,
            IsAdmin = model.IsAdmin,
        };
    }
}