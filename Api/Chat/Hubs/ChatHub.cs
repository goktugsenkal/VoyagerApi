using Core.Interfaces;
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
        var user = Context.User?.Identity?.Name ?? Context.ConnectionId;
        // Notify everyone that this user is online
        await Clients.Others.SendAsync("UserOnline", user);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects. Broadcasts presence to all users.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = Context.User?.Identity?.Name ?? Context.ConnectionId;
        // Notify everyone that this user went offline
        await Clients.Others.SendAsync("UserOffline", user);

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
    /// <param name="roomName">Logical name of the chat room.</param>
    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

        // Track membership
        if (!_groupMembers.ContainsKey(roomName))
            _groupMembers[roomName] = new List<string>();
        _groupMembers[roomName].Add(Context.ConnectionId);

        // Notify others in the room
        await Clients.Group(roomName).SendAsync("UserJoined", roomName, Context.ConnectionId);
    }

    /// <summary>
    /// Leave a chat room (group).  
    /// Removes the caller from the group and membership tracking.
    /// </summary>
    /// <param name="roomName">Logical name of the chat room.</param>
    public async Task LeaveRoom(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

        if (_groupMembers.ContainsKey(roomName))
            _groupMembers[roomName].Remove(Context.ConnectionId);

        await Clients.Group(roomName).SendAsync("UserLeft", roomName, Context.ConnectionId);
    }

    /// <summary>
    /// Send a message to everyone in a room.
    /// </summary>
    /// <param name="roomName">Target room name.</param>
    /// <param name="user">Sender identifier.</param>
    /// <param name="message">Message text.</param>
    public async Task SendMessage(string roomName, string user, string message)
    {
        // Persist message to DB here if required
        await Clients.Group(roomName)
                     .SendAsync("ReceiveMessage", roomName, user, message, DateTime.UtcNow);
    }

    /// <summary>
    /// Send a private one‑to‑one message.
    /// </summary>
    /// <param name="targetConnectionId">Connection ID of the recipient.</param>
    /// <param name="fromUser">Sender identifier.</param>
    /// <param name="message">Message text.</param>
    public async Task SendPrivateMessage(string targetConnectionId, string fromUser, string message)
    {
        await Clients.Client(targetConnectionId)
                     .SendAsync("ReceivePrivateMessage", Context.ConnectionId, fromUser, message, DateTime.UtcNow);
    }

    /// <summary>
    /// Notify others in the room that the caller is typing.
    /// </summary>
    /// <param name="roomName">Room where typing is happening.</param>
    /// <param name="user">User who is typing.</param>
    public async Task Typing(string roomName, string user)
        => await Clients.Group(roomName)
                        .SendAsync("UserTyping", roomName, user);

    /// <summary>
    /// Send a read receipt for a given message in a room.
    /// </summary>
    /// <param name="roomName">Room in which the message was read.</param>
    /// <param name="messageId">Identifier of the message.</param>
    /// <param name="user">User who read it.</param>
    public async Task SendReadReceipt(string roomName, string messageId, string user)
        => await Clients.Group(roomName)
                        .SendAsync("MessageRead", roomName, messageId, user, DateTime.UtcNow);

    /// <summary>
    /// Request the list of users currently in a room.
    /// </summary>
    /// <param name="roomName">Target room name.</param>
    /// <returns>List of connection IDs in that room.</returns>
    public Task<List<string>> GetGroupMembers(string roomName)
    {
        _groupMembers.TryGetValue(roomName, out var members);
        return Task.FromResult(members ?? new List<string>());
    }

    /// <summary>
    /// Edit a previously sent message.
    /// </summary>
    /// <param name="roomName">Room where the message resides.</param>
    /// <param name="messageId">Identifier of the message.</param>
    /// <param name="newText">New text for the message.</param>
    public async Task EditMessage(string roomName, string messageId, string newText)
    {
        // Update in database first...
        await Clients.Group(roomName)
                     .SendAsync("MessageEdited", roomName, messageId, newText, DateTime.UtcNow);
    }

    /// <summary>
    /// Delete a message in a room.
    /// </summary>
    /// <param name="roomName">Room where the message resides.</param>
    /// <param name="messageId">Identifier of the message.</param>
    public async Task DeleteMessage(string roomName, string messageId)
    {
        // Remove from database first...
        await Clients.Group(roomName)
                     .SendAsync("MessageDeleted", roomName, messageId);
    }
}
