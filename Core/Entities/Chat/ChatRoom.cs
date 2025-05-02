using Core.Dtos.Chat;
using Core.Models.Chat;

namespace Core.Entities.Chat;

public class ChatRoom : BaseEntity
{
    // BaseEntity provides Id (GUID) and CreatedAt

    /// <summary>
    /// distinguishes between 1:1 or group chats
    /// </summary>
    public ChatRoomType Type { get; set; }

    /// <summary>
    /// user decided name for the chat
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// user decided name for the chat
    /// </summary>
    public required string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// user decided name for the chat
    /// </summary>
    public string ImageKey { get; set; } = "default/group.png";

    /// <summary>
    /// user decided name for the chat
    /// </summary>
    public string BannerKey { get; set; } = "default/banner.png";
    
    /// <summary>
    /// navigation property for the messages in the chat.
    /// </summary>
    public ICollection<Message> Messages { get; set; } = [];

    /// <summary>
    /// navigation property for the participants of the chat.
    /// </summary>
    public ICollection<ChatRoomParticipant> Participants { get; set; } = [];
}

public enum ChatRoomType
{
    Private = 0,
    Group   = 1
}

public static class ChatRoomTypeExtensions
{
    public static ChatRoomDto ToDto(this ChatRoom room)
    {
        return new ChatRoomDto
        {
            Id          = room.Id,
            CreatedAt   = room.CreatedAt,
            Title       = room.Title,
            Description = room.Description,
            Type        = room.Type.ToString(),
            // last message will be added later
        };
    }
    
    // create chat room model -> chat room
    public static ChatRoom ToChatRoom(this CreateChatRoomModel model)
    {
        return new ChatRoom
        {
            Type = model.Type,
            Title = model.Title,
            Description = model.Description,
        };
    }
}