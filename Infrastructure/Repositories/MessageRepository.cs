using Core.Entities.Chat;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MessageRepository(DataContext context) : IMessageRepository
{
    public async Task AddAsync(Message message)
    {
        message.CreatedAt = DateTime.UtcNow;
        message.UpdatedAt = DateTime.UtcNow;
        await context.ChatMessages.AddAsync(message);
        await SaveChangesAsync();
    }

    public async Task RemoveAsync(Message message)
    {
        context.ChatMessages.Remove(message);
        await SaveChangesAsync();
    }
    
    public async Task<Message?> GetByIdAsync(Guid messageId)
    {
        return await context.ChatMessages.FirstOrDefaultAsync(m => m.Id == messageId);
    }

    public async Task<ChatMessageReadReceipt?> GetReadReceiptAsync(Guid messageId, Guid userId)
    {
        return await context.ChatMessageReadReceipts
            .FirstOrDefaultAsync(r => r.ClientMessageId == messageId && r.UserId == userId);
    }

    public async Task AddReadReceiptAsync(ChatMessageReadReceipt receipt)
    {
        receipt.CreatedAt = DateTime.UtcNow;
        await context.ChatMessageReadReceipts.AddAsync(receipt);
        await SaveChangesAsync();
    }

    public async Task<ChatMessageDeliveredReceipt?> GetDeliveredReceiptAsync(Guid messageId, Guid userId)
    {
        return await context.ChatMessageDeliveredReceipts
            .FirstOrDefaultAsync(r => r.ClientMessageId == messageId && r.UserId == userId);
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