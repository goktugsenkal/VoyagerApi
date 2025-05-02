using System.Security.Claims;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace Api.Chat.Hubs;

public class ChatHub(IChatService chatService) : Hub
{
    // In-memory store for demo purposes only.
    // In production, back this with a database or distributed cache.
    private static readonly Dictionary<string, List<string>> _groupMembers = new();

    /// <summary>
    /// Called when a new client connects. Broadcasts presence to all users.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        // Get user ID
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                     ?? Context.ConnectionId;
        
        // figure out what the fuck rooms and groups are
            
        // Notify everyone that this user came online
        await Clients.Others.SendAsync("UserOnline", userId);
        
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects. Broadcasts presence to all users.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Get user ID
        var userId = Context.User?.Identity?.Name ?? Context.ConnectionId;
        // Notify everyone that this user went offline
        await Clients.Others.SendAsync("UserOffline", userId);

        // Clean up from any groups they were in
        foreach (var kvp in _groupMembers)
        {
            if (kvp.Value.Contains(Context.ConnectionId))
                kvp.Value.Remove(Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join a chat room (group).  
    /// Adds the caller to the group and tracks membership.
    /// </summary>
    /// <param name="roomId">Pragmatically id based name of the chat room.</param>
    public async Task JoinRoom(string roomId)
    {
        // Join the group
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

        // Track membership
        if (!_groupMembers.ContainsKey(roomId))
            _groupMembers[roomId] = new List<string>();
        _groupMembers[roomId].Add(Context.ConnectionId);

        // Notify others in the room
        await Clients.Group(roomId).SendAsync("UserJoined", roomId, Context.ConnectionId);
    }

    /// <summary>
    /// Leave a chat room (group).  
    /// Removes the caller from the group and membership tracking.
    /// </summary>
    /// <param name="roomId">Logical name of the chat room.</param>
    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

        if (_groupMembers.ContainsKey(roomId))
            _groupMembers[roomId].Remove(Context.ConnectionId);

        await Clients.Group(roomId).SendAsync("UserLeft", roomId, Context.ConnectionId);
    }

    /// <summary>
    /// Send a message to everyone in a room.
    /// </summary>
    /// <param name="roomId">Target room name.</param>
    /// <param name="userId">Sender identifier.</param>
    /// <param name="messageId">global message id</param>
    /// <param name="message">Message text.</param>
    /// <param name="voyageId">If the message contains a voyage, its ID will be included</param>
    public async Task SendMessage(string roomId, string userId, string messageId, string message, string? voyageId = null)
    {
        // if voyageId is not null, then the message contains a voyage
        if (voyageId is not null)
        {
            // send message with voyageId
            await Clients.Group(roomId)
                .SendAsync("MessageReceived", roomId, userId, messageId, message, DateTime.UtcNow, voyageId);
        }
        // if voyageId is null, then the message does not contain a voyage
        else
        {
            // send message
            await Clients.Group(roomId)
                .SendAsync("MessageReceived", roomId, userId, messageId, message, DateTime.UtcNow);
        }
    }

    /// <summary>
    /// Notify others in the room that the caller is typing.
    /// </summary>
    /// <param name="roomId">Room where typing is happening.</param>
    /// <param name="userId">User who is typing.</param>
    public async Task StartTyping(string roomId, string userId)
        => await Clients.Group(roomId)
                        .SendAsync("UserTyping", roomId, userId);
    
    /// <summary>
    /// Notify others in the room that the caller is typing.
    /// </summary>
    /// <param name="roomId">Room where typing is happening.</param>
    /// <param name="userId">User who is typing.</param>
    public async Task StopTyping(string roomId, string userId)
        => await Clients.Group(roomId)
                        .SendAsync("UserStoppedTyping", roomId, userId);
    

    /// <summary>
    /// Send a read receipt for a given message in a room.
    /// </summary>
    /// <param name="roomId">Room in which the message was read.</param>
    /// <param name="messageId">Identifier of the message.</param>
    /// <param name="userId">User who read it. Good for group chats.</param>
    public async Task MarkAsRead(string roomId, string messageId, string userId)
        => await Clients.Group(roomId)
                        .SendAsync("ReadReceipt", roomId, messageId, userId, DateTime.UtcNow);

    /// <summary>
    /// Request the list of users currently in a room.
    /// </summary>
    /// <param name="roomId">Target room name.</param>
    /// <returns>List of connection IDs in that room.</returns>
    public Task<List<string>> GetGroupMembers(string roomId)
    {
        _groupMembers.TryGetValue(roomId, out var members);
        return Task.FromResult(members ?? new List<string>());
    }

    /// <summary>
    /// Edit a previously sent message.
    /// </summary>
    /// <param name="roomId">Room where the message resides.</param>
    /// <param name="messageId">Identifier of the message.</param>
    /// <param name="newMessage">New text for the message.</param>
    public async Task EditMessage(string roomId, string messageId, string newMessage)
    {
        // Update in database first...
        await Clients.Group(roomId)
                     .SendAsync("MessageEdited", roomId, messageId, newMessage, DateTime.UtcNow);
    }

    /// <summary>
    /// Delete a message in a room.
    /// </summary>
    /// <param name="roomId">Room where the message resides.</param>
    /// <param name="userId">Which user deleted the message (the sender of the message, an admin, ...). </param>
    /// <param name="messageId">Identifier of the message.</param>
    public async Task MarkAsDeleted(string roomId,string userId, string messageId)
    {
        // Remove from database first...
        await Clients.Group(roomId)
                     .SendAsync("DeletedReceipt", roomId, messageId);
    }
}
