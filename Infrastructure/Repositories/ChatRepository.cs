using Core.Entities.Chat;
using Core.Interfaces.Repositories;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ChatRepository(DataContext context) : IChatRepository
{
    public async Task<ChatUser?> GetChatUserByIdAsync(Guid userId)
    {
        return await context.ChatUsers.FirstOrDefaultAsync(u => u.Id == userId);
    }
    
    public async Task<bool> ChatUserExistsAsync(Guid userId)
    {
        return await context.ChatUsers.AnyAsync(u => u.Id == userId);
    }

    public async Task AddChatUserAsync(ChatUser user)
    {
        user.CreatedAt = DateTime.UtcNow;
        await context.ChatUsers.AddAsync(user);
        await SaveChangesAsync();
    }

    public async Task UpdateChatUserAsync(ChatUser user)
    {
        throw new NotImplementedException();
    }

    public async Task<ChatRoom?> GetChatRoomByIdAsync(Guid roomId)
    {
        return await context.ChatRooms.FirstOrDefaultAsync(r => r.Id == roomId);
    }

    public async Task<PagedList<ChatRoom>> GetChatRoomsForUserAsync(Guid userId, int pageNumber = 1, int pageSize = 20)
    {
        var query = context.ChatRoomParticipants
            .Where(p => p.UserId == userId)
            .Include(p => p.ChatRoom)
            .Select(p => p.ChatRoom);

        return PagedList<ChatRoom>.CreatePagedList(query, pageNumber, pageSize);
    }

    public async Task AddChatRoomAsync(ChatRoom room)
    {
        room.CreatedAt = DateTime.UtcNow;
        await context.ChatRooms.AddAsync(room);
        await SaveChangesAsync();
    }

    public async Task UpdateChatRoomAsync(ChatRoom room)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteChatRoomAsync(ChatRoom room)
    {
        throw new NotImplementedException();
    }

    public async Task<ChatRoomParticipant?> GetParticipantAsync(Guid roomId, Guid userId)
    {
       return await context.ChatRoomParticipants
           .FirstOrDefaultAsync(p => p.ChatRoomId == roomId && p.UserId == userId); 
    }

    public async Task<List<ChatRoomParticipant>> GetParticipantsAsync(Guid roomId)
    {
        return await context.ChatRoomParticipants.Where(p => p.ChatRoomId == roomId).ToListAsync();
    }

    public async Task AddParticipantAsync(ChatRoomParticipant participant)
    {
        participant.CreatedAt = DateTime.UtcNow;
        await context.ChatRoomParticipants.AddAsync(participant);
        await SaveChangesAsync();
    }

    public async Task RemoveParticipantAsync(ChatRoomParticipant participant)
    {
        context.ChatRoomParticipants.Remove(participant);
        await SaveChangesAsync();
    }

    public async Task<Message?> GetMessageByIdAsync(Guid messageId)
    {
        return await context.ChatMessages.FirstOrDefaultAsync(m => m.Id == messageId);
    }

    public async Task<List<Message>> GetMessagesAsync(Guid roomId, DateTime? before, int limit)
    {
        throw new NotImplementedException();
    }

    public async Task AddMessageAsync(Message message)
    {
        message.CreatedAt = DateTime.UtcNow;
        await context.ChatMessages.AddAsync(message);
        await SaveChangesAsync();
    }

    public async Task<Message?> GetLastMessageForARoomAsync(Guid roomId)
    {
        return await context.ChatMessages
            .Where(m => m.ChatRoomId == roomId)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> HasReadReceiptAsync(Guid messageId, Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task AddReadReceiptAsync(ChatMessageReadReceipt receipt)
    {
        receipt.CreatedAt = DateTime.UtcNow;
        await context.ChatMessageReadReceipts.AddAsync(receipt);
        await SaveChangesAsync();
    }

    public async Task<bool> HasDeliveredReceiptAsync(Guid messageId, Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task AddDeliveredReceiptAsync(ChatMessageDeliveredReceipt receipt)
    {
        receipt.CreatedAt = DateTime.UtcNow;
        await context.ChatMessageDeliveredReceipts.AddAsync(receipt);
        await SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}