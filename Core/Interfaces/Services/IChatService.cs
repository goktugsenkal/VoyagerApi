using Core.Dtos.Chat;
using Core.Entities.Chat;
using Core.Models;
using Core.Models.Chat;
using Core.Results;

namespace Core.Interfaces.Services;

public interface IChatService
{
    Task SignUpForChatAsync(Guid userId, SignUpForChatModel model);
    Task<bool> IsUserSignedUpForChatAsync(Guid userId);
    Task<CreateChatRoomResult> CreateChatRoomAsync(CreateChatRoomModel roomModel);
    Task<PagedList<ChatRoomDto>> GetChatRoomsForUserAsync(Guid userId, int pageNumber, int pageSize);
    Task<List<Guid>> GetChatRoomIdsForUserAsync(Guid userId);
    Task<List<Guid>> GetParticipantsForChatRoomAsync(Guid roomId);
    Task<List<Guid>> GetOnlinePeersAsyncForUserAsync(Guid userId);
    Task AddChatRoomParticipantAsync(Guid roomId, CreateChatRoomParticipantModel participantModel);
    
    // chat hub methods
    Task SaveMessageAsync(Guid messageId, Guid roomId, Guid senderId, string text, Guid? voyageId = null);
    Task EditMessageAsync(Guid messageId, string newText);
    Task DeleteMessageAsync(Guid messageId);
    Task MarkMessageAsReadAsync(Guid messageId, Guid readerId);
    Task MarkMessageAsDeliveredAsync(Guid messageId, Guid userId);
}
