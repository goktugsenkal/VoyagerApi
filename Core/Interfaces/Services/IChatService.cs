using Core.Entities.Chat;
using Core.Models.Chat;
using Core.Results;

namespace Core.Interfaces.Services;

public interface IChatService
{
    Task SignUpForChatAsync(Guid userId);
    Task<CreateChatRoomResult> CreateChatRoomAsync(CreateChatRoomModel roomModel);
}
