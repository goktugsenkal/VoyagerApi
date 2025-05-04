using Core.Entities.Chat;
using Core.Models;

namespace Core.Interfaces.Repositories;

public interface IChatRepository
{
    // ChatUser
    Task<ChatUser?> GetChatUserByIdAsync(Guid userId);
    Task<bool> ChatUserExistsAsync(Guid userId);
    Task AddChatUserAsync(ChatUser user);
    Task UpdateChatUserAsync(ChatUser user);

    // ChatRoom
    Task<ChatRoom?> GetChatRoomByIdAsync(Guid roomId);
    Task<PagedList<ChatRoom>> GetChatRoomsForUserAsync(Guid userId, int pageNumber, int pageSize);
    Task<List<Guid>> GetChatRoomIdsForUserAsync(Guid userId);
    Task AddChatRoomAsync(ChatRoom room);
    Task UpdateChatRoomAsync(ChatRoom room);
    Task DeleteChatRoomAsync(ChatRoom room);

    // ChatRoomParticipant
    Task<ChatRoomParticipant?> GetParticipantAsync(Guid roomId, Guid userId);
    Task<List<Guid>> GetParticipantIdsForRoomAsync(Guid roomId);
    Task<List<ChatRoomParticipant>> GetParticipantsAsync(Guid roomId);
    Task AddParticipantAsync(ChatRoomParticipant participant);
    Task RemoveParticipantAsync(ChatRoomParticipant participant);

    // Message
    Task<Message?> GetMessageByIdAsync(Guid messageId);
    Task<List<Message>> GetMessagesAsync(Guid roomId, DateTime? before, int limit);
    Task AddMessageAsync(Message message);
    Task<Message?> GetLastMessageForARoomAsync(Guid roomId);

    // Read Receipt
    Task<bool> HasReadReceiptAsync(Guid messageId, Guid userId);
    Task AddReadReceiptAsync(ChatMessageReadReceipt receipt);

    // Delivery Receipt
    Task<bool> HasDeliveredReceiptAsync(Guid messageId, Guid userId);
    Task AddDeliveredReceiptAsync(ChatMessageDeliveredReceipt receipt);

    // Commit
    Task SaveChangesAsync();
}