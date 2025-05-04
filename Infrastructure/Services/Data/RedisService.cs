using Core.Interfaces.Data;

namespace Infrastructure.Services.Data;

using StackExchange.Redis;
using System.Text.Json;

public class RedisService(IConnectionMultiplexer multiplexer) : IRedisService
{
    private readonly IDatabase _db = multiplexer.GetDatabase();

    // presence
    private const string OnlineUsersKey = "online_users";

    public Task AddOnlineUserAsync(Guid userId) 
        => _db.SetAddAsync(OnlineUsersKey, userId.ToString());

    public Task RemoveOnlineUserAsync(Guid userId) 
        => _db.SetRemoveAsync(OnlineUsersKey, userId.ToString());

    public Task<bool> IsUserOnlineAsync(Guid userId) 
        => _db.SetContainsAsync(OnlineUsersKey, userId.ToString());

    public async Task<List<Guid>> GetAllOnlineUsersAsync()
    {
        var members = await _db.SetMembersAsync(OnlineUsersKey);
        return members.Select(rv => Guid.Parse(rv!)).ToList();
    }

    // chat-room caching
    private static string ChatRoomsKey(Guid userId) 
        => $"chat-rooms:{userId}";

    public async Task<List<Guid>> GetCachedChatRoomIdsAsync(Guid userId)
    {
        var raw = await _db.StringGetAsync(ChatRoomsKey(userId));
        if (raw.IsNullOrEmpty) 
            return new List<Guid>();
        return JsonSerializer.Deserialize<List<Guid>>(raw!)!;
    }

    public Task CacheChatRoomIdsAsync(Guid userId, List<Guid> roomIds, TimeSpan? ttl = null)
    {
        var payload = JsonSerializer.Serialize(roomIds);
        return _db.StringSetAsync(
            ChatRoomsKey(userId), 
            payload, 
            expiry: ttl ?? TimeSpan.FromMinutes(60));
    }

    public Task InvalidateChatRoomCacheAsync(Guid userId) 
        => _db.KeyDeleteAsync(ChatRoomsKey(userId));
}