using Core.Entities.Chat;

namespace Core.Interfaces.Repositories;

public interface IMessageRepository
{
    Task AddAsync(Message message);
    Task RemoveAsync(Message message);
    Task<Message?> GetByIdAsync(Guid messageId);
    Task<ChatMessageReadReceipt?> GetReadReceiptAsync(Guid messageId, Guid userId);
    Task AddReadReceiptAsync(ChatMessageReadReceipt receipt);
    Task<ChatMessageDeliveredReceipt?> GetDeliveredReceiptAsync(Guid messageId, Guid userId);
    Task AddDeliveredReceiptAsync(ChatMessageDeliveredReceipt receipt);
    Task SaveChangesAsync();
}