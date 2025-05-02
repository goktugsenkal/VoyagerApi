using Core.Entities.Chat;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models.Chat;
using Core.Results;

namespace Infrastructure.Services;

public class ChatService(IChatRepository chatRepository, IS3Service s3Service) : IChatService
{
    public async Task<CreateChatRoomResult> CreateChatRoomAsync(CreateChatRoomModel roomModel)
    {
        // todo: if p2p, type = 0, if group, type = 1
        
        // map the CreateChatRoomModel to a ChatRoom
        var chatRoom = roomModel.ToChatRoom();

        // save to the database to get the ID
        await chatRepository.AddChatRoomAsync(chatRoom);

        // add participants
        foreach (var participantModel in roomModel.ParticipantModels)
        {
            var participant = participantModel.ToEntity(chatRoom.Id);
            await chatRepository.AddParticipantAsync(participant);
        }

        string? imageUploadUrl = null;
        string? bannerUploadUrl = null;

        // image and banner types (jpg, png, etc) are validated at the controller level
        if (!string.IsNullOrEmpty(roomModel.ImageType))
        {
            var imageKey = $"chat/rooms/{chatRoom.Id}/{Guid.NewGuid()}.{roomModel.ImageType}";
            imageUploadUrl = s3Service.GeneratePreSignedUploadUrl(imageKey, TimeSpan.FromMinutes(15));
            chatRoom.ImageKey = imageKey;
        }

        if (!string.IsNullOrEmpty(roomModel.BannerType))
        {
            var bannerKey = $"chat/rooms/{chatRoom.Id}/{Guid.NewGuid()}.{roomModel.BannerType}";
            bannerUploadUrl = s3Service.GeneratePreSignedUploadUrl(bannerKey, TimeSpan.FromMinutes(15));
            chatRoom.BannerKey = bannerKey;
        }

        await chatRepository.SaveChangesAsync();

        return new CreateChatRoomResult(imageUploadUrl, bannerUploadUrl);
    }
    
    public async Task SignUpForChatAsync(Guid userId)
    {
        if (await chatRepository.ChatUserExistsAsync(userId))
            throw new InvalidOperationException("User is already registered for chat.");

        var chatUser = new ChatUser
        {
            Id = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AllowDMs = true,
            ShowLastSeen = true,
            LastSeen = DateTime.UtcNow
        };

        await chatRepository.AddChatUserAsync(chatUser);
        await chatRepository.SaveChangesAsync();
    }
}
