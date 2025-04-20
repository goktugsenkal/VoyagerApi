using Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services;

public class ChatService : IChatService
{
    public Task SaveMessageAsync(Guid userId, string message)
    {
        // save to db or whatever
        return Task.CompletedTask;
    }
}
