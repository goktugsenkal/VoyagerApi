namespace Core.Interfaces.Data;

public interface IRedisService
{
    // ––––––––– Presence
    Task AddOnlineUserAsync(Guid userId);
    Task RemoveOnlineUserAsync(Guid userId);
    Task<bool> IsUserOnlineAsync(Guid userId);
    Task<List<Guid>> GetAllOnlineUsersAsync();

    // ––––––––– Chat-room caching
    Task<List<Guid>> GetCachedChatRoomIdsAsync(Guid userId);
    Task CacheChatRoomIdsAsync(Guid userId, List<Guid> roomIds, TimeSpan? ttl = null);
    Task InvalidateChatRoomCacheAsync(Guid userId);
}