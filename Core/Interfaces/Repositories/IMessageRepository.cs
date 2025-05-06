using Core.Entities.Chat;

namespace Core.Interfaces.Repositories;

public interface IMessageRepository
{
    Task AddAsync(Message message);
    Task RemoveAsync(Message message);
    Task<Message?> GetByIdAsync(Guid messageId);
    
    Task AddReadReceiptAsync(Guid messageId, Guid userId);
    Task AddDeliveredReceiptAsync(Guid messageId, Guid userId);
    Task SaveChangesAsync();
}