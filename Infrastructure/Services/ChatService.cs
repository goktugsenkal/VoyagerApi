using Core.Dtos.Chat;
using Core.Entities.Chat;
using Core.Interfaces;
using Core.Interfaces.Data;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Core.Models.Chat;
using Core.Results;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class ChatService(IChatRepository chatRepository, 
    IS3Service s3Service, 
    IMessageRepository messageRepository,
    IUserRepository userRepository, 
    IRedisService redisService) : IChatService
{
    public async Task SignUpForChatAsync(Guid userId, SignUpForChatModel model)
    {
        if (await chatRepository.ChatUserExistsAsync(userId))
            throw new InvalidOperationException("User is already registered for chat.");
        
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException("User not found.");

        var chatUser = new ChatUser
        {
            Id = userId,
            DisplayName = model.DisplayName,
            StatusMessage = model.StatusMessage,
            AllowDMs = model.AllowDMs,
            ShowLastSeen = model.ShowLastSeen,
            ShowOnline = model.ShowOnline,
            LastSeen = DateTime.UtcNow
        };

        await chatRepository.AddChatUserAsync(chatUser);
        await chatRepository.SaveChangesAsync();
    }

    public async Task<bool> IsUserSignedUpForChatAsync(Guid userId)
    {
        return await chatRepository.ChatUserExistsAsync(userId);
    }

    public async Task<CreateChatRoomResult> CreateChatRoomAsync(CreateChatRoomModel roomModel)
    {
        // map the CreateChatRoomModel to a ChatRoom
        var chatRoom = roomModel.ToChatRoom();

        // save to the database to get the ID
        await chatRepository.AddChatRoomAsync(chatRoom);

        // add participants
        foreach (var participantModel in roomModel.ParticipantModels)
        {
            if (await chatRepository.ChatUserExistsAsync(participantModel.UserId))
            {
                var participant = participantModel.ToEntity(chatRoom.Id);
                await chatRepository.AddParticipantAsync(participant);
            }
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
    
    public async Task<PagedList<ChatRoomDto>> GetChatRoomsForUserAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 20)
    {
        // 1. Fetch the paged list of ChatRoom entities
        var pagedRooms = await chatRepository
            .GetChatRoomsForUserAsync(userId, pageNumber, pageSize);

        // 2. Map each ChatRoom to ChatRoomDto
        var roomDtos = new List<ChatRoomDto>();
        foreach (var room in pagedRooms.Items)
        {
            var dto = room.ToDto();
            var lastMsg = await chatRepository.GetLastMessageForARoomAsync(room.Id);
            dto.LastMessage = lastMsg?.ToDto();
            roomDtos.Add(dto);
        }

        // 3. Return a new PagedList of DTOs
        return new PagedList<ChatRoomDto>(
            roomDtos,
            pagedRooms.TotalCount,
            pagedRooms.CurrentPage,
            pagedRooms.PageSize
        );
    }

    public async Task<List<Guid>> GetChatRoomIdsForUserAsync(Guid userId)
    {
        var cached = await redisService.GetCachedChatRoomIdsAsync(userId);
        if (cached.Count > 0) return cached;

        var roomIds = await chatRepository.GetChatRoomIdsForUserAsync(userId);

        await redisService.CacheChatRoomIdsAsync(userId, roomIds);
        return roomIds;
    }

    public async Task<List<Guid>> GetOnlinePeersAsyncForUserAsync(Guid userId)
    {
        // 1) get all room IDs (cached + DB fallback)
        var roomIds = await GetChatRoomIdsForUserAsync(userId);
        if (!roomIds.Any())
            return new List<Guid>();

        // 2) grab the entire set of online users in one go
        var onlineSet = new HashSet<Guid>(await redisService.GetAllOnlineUsersAsync());

        var peers = new HashSet<Guid>();
        foreach (var roomId in roomIds)
        {
            // 3) fetch participants for that room
            var participants = await chatRepository.GetParticipantIdsForRoomAsync(roomId);

            // 4) only consider 1:1 rooms
            if (participants.Count == 2)
            {
                // the “peer” is the one that isn’t `userId`
                var peerId = participants[0] == userId 
                    ? participants[1] 
                    : participants[0];

                // 5) if they’re online, add to result
                if (onlineSet.Contains(peerId))
                    peers.Add(peerId);
            }
        }

        return peers.ToList();
    }

    public async Task AddChatRoomParticipantAsync(Guid roomId, CreateChatRoomParticipantModel participantModel)
    {
        if (!await chatRepository.ChatUserExistsAsync(participantModel.UserId))
        {
            throw new KeyNotFoundException("User not found or not registered for chat.");
        }
        
        var participant = participantModel.ToEntity(roomId);
        await chatRepository.AddParticipantAsync(participant);
        await chatRepository.SaveChangesAsync();
    }

    // ________________
    // chat hub methods
    // ________________
    public async Task SaveMessageAsync(Guid messageId, Guid roomId, Guid senderId, string text, Guid? voyageId = null)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ClientMessageId = messageId,
            ChatRoomId = roomId,
            SenderId = senderId,
            Text = text,
            VoyageId = voyageId,
            CreatedAt = DateTime.UtcNow
        };

        await messageRepository.AddAsync(message);
        await messageRepository.SaveChangesAsync();
    }

    public async Task DeleteMessageAsync(Guid messageId)
    {
        var message = await messageRepository.GetByIdAsync(messageId);
        if (message == null) return;

        await messageRepository.RemoveAsync(message);
        await messageRepository.SaveChangesAsync();
    }

    public async Task EditMessageAsync(Guid messageId, string newText)
    {
        throw new NotImplementedException();
    }

    public async Task MarkMessageAsReadAsync(Guid messageId, Guid readerId)
    {
        await messageRepository.AddReadReceiptAsync(messageId, readerId);
    }

    public async Task MarkMessageAsDeliveredAsync(Guid messageId, Guid receiverId)
    {
        await messageRepository.AddDeliveredReceiptAsync(messageId, receiverId);
    }
}
