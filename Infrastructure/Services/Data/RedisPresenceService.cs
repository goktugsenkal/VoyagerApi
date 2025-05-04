using StackExchange.Redis;

namespace Infrastructure.Services.Data;

public class RedisPresenceService(IConnectionMultiplexer redis)
{
    private readonly IDatabase _db = redis.GetDatabase();
    private const string OnlineUsersSetKey = "online_users";

    public Task AddOnlineUserAsync(Guid userId)
        => _db.SetAddAsync(OnlineUsersSetKey, userId.ToString());

    public Task RemoveOnlineUserAsync(Guid userId)
        => _db.SetRemoveAsync(OnlineUsersSetKey, userId.ToString());

    public Task<bool> IsUserOnlineAsync(Guid userId)
        => _db.SetContainsAsync(OnlineUsersSetKey, userId.ToString());

    public async Task<List<Guid>> GetAllOnlineUsersAsync()
    {
        var members = await _db.SetMembersAsync(OnlineUsersSetKey);
        return members.Select(x => Guid.Parse(x!)).ToList();
    }
}