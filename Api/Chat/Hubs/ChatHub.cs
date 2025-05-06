using System.Security.Claims;
using Core.Interfaces;
using Core.Interfaces.Data;
using Core.Interfaces.Services;
using Infrastructure.Services.Data;
using Microsoft.AspNetCore.SignalR;

namespace Api.Chat.Hubs;

public class ChatHub(IChatService chatService, IRedisService redisService, ILogger<ChatHub> logger) : Hub
{
    // // In-memory store for demo purposes only.
    // // In production, back this with a database or distributed cache.
    // private static readonly Dictionary<string, List<string>> _groupMembers = new();

    /// <summary>
    /// Called when a new client connects. Broadcasts presence to all users.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        // Extract user ID from JWT claims
        var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            Context.Abort();
            return;
        }

        // Add to Redis presence
        await redisService.AddOnlineUserAsync(userId);

        // Automatically join all chat rooms user is a member of
        var chatRoomIds = await chatService.GetChatRoomIdsForUserAsync(userId);
        foreach (var roomId in chatRoomIds)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        }

        // Notify others user is online
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

        if (Guid.TryParse(userId, out var guid))
            await redisService.RemoveOnlineUserAsync(guid);

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

        // // Track membership
        // if (!_groupMembers.ContainsKey(roomId))
        //     _groupMembers[roomId] = new List<string>();
        // _groupMembers[roomId].Add(Context.ConnectionId);

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

        // if (_groupMembers.ContainsKey(roomId))
        //     _groupMembers[roomId].Remove(Context.ConnectionId);

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
        logger.LogInformation("[SendMessage] raw input - roomId: {RoomId}, userId: {UserId}, messageId: {MessageId}, message: {Message}, voyageId: {VoyageId}", roomId, userId, messageId, message, voyageId);

        Guid parsedMessageId, parsedRoomId, parsedUserId;
        DateTime timestamp = DateTime.UtcNow;

        try
        {
            parsedMessageId = Guid.Parse(messageId);
            logger.LogInformation("[SendMessage] parsedMessageId: {ParsedMessageId}", parsedMessageId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[SendMessage] failed to parse messageId: {MessageId}", messageId);
            throw;
        }

        try
        {
            parsedRoomId = Guid.Parse(roomId);
            logger.LogInformation("[SendMessage] parsedRoomId: {ParsedRoomId}", parsedRoomId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[SendMessage] failed to parse roomId: {RoomId}", roomId);
            throw;
        }

        try
        {
            parsedUserId = Guid.Parse(userId);
            logger.LogInformation("[SendMessage] parsedUserId: {ParsedUserId}", parsedUserId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[SendMessage] failed to parse userId: {UserId}", userId);
            throw;
        }

        logger.LogInformation("[SendMessage] timestamp: {Timestamp}", timestamp);

        if (!string.IsNullOrEmpty(voyageId))
        {
            logger.LogInformation("[SendMessage] message includes a voyageId: {VoyageId}", voyageId);
            
            await Clients.OthersInGroup(roomId).SendAsync("MessageReceived", roomId, userId, messageId, message, timestamp, voyageId);
            logger.LogInformation("[SendMessage] sent MessageReceived event to OthersInGroup with voyageId");

            await chatService.SaveMessageAsync(parsedMessageId, parsedRoomId, parsedUserId, message, Guid.Parse(voyageId));
            logger.LogInformation("[SendMessage] saved message with voyageId to chatService");

            await chatService.MarkMessageAsDeliveredAsync(parsedMessageId, parsedUserId);
            logger.LogInformation("[SendMessage] marked message as delivered with voyageId");
        }
        else
        {
            logger.LogInformation("[SendMessage] message has no voyageId");
            
            await Clients.OthersInGroup(roomId).SendAsync("MessageReceived", roomId, userId, messageId, message, timestamp);
            logger.LogInformation("[SendMessage] sent MessageReceived event to OthersInGroup without voyageId");

            await chatService.SaveMessageAsync(parsedMessageId, parsedRoomId, parsedUserId, message);
            logger.LogInformation("[SendMessage] saved message without voyageId to chatService");

            await chatService.MarkMessageAsDeliveredAsync(parsedMessageId, parsedUserId);
            logger.LogInformation("[SendMessage] marked message as delivered without voyageId");
        }
    }

    /// <summary>
    /// Notify others in the room that the caller is typing.
    /// </summary>
    /// <param name="roomId">Room where typing is happening.</param>
    /// <param name="userId">User who is typing.</param>
    public async Task StartTyping(string roomId, string userId)
    {
        await Clients.Group(roomId)
            .SendAsync("UserTyping", roomId, userId);
    }

    /// <summary>
    /// Notify others in the room that the caller is typing.
    /// </summary>
    /// <param name="roomId">Room where typing is happening.</param>
    /// <param name="userId">User who is typing.</param>
    public async Task StopTyping(string roomId, string userId)
    {
        await Clients.Group(roomId)
            .SendAsync("UserStoppedTyping", roomId, userId);
    }

    /// <summary>
    /// Send a read receipt for a given message in a room.
    /// </summary>
    /// <param name="roomId">Room in which the message was read.</param>
    /// <param name="messageId">Identifier of the message.</param>
    /// <param name="userId">User who read it. Good for group chats.</param>
    public async Task MarkAsRead(string roomId, string messageId, string userId)
    {
        var parsedMessageId = Guid.Parse(messageId);
        var parsedUserId = Guid.Parse(userId);
        
        await Clients.Group(roomId)
            .SendAsync("ReadReceipt", roomId, messageId, userId, DateTime.UtcNow);
        
        await chatService.MarkMessageAsReadAsync(parsedMessageId, parsedUserId);
    }

    /// <summary>
    /// Request the list of users currently in a room.
    /// </summary>
    /// <param name="roomId">Target room name.</param>
    /// <returns>List of connection IDs in that room.</returns>
    public Task<List<string>> GetGroupMembers(string roomId)
    {
        // _groupMembers.TryGetValue(roomId, out var members);
        // return Task.FromResult(members ?? new List<string>());
        throw new NotImplementedException();
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
        var parsedMessageId = Guid.Parse(messageId);
        
        await chatService.DeleteMessageAsync(parsedMessageId);
        
        await Clients.Group(roomId)
                     .SendAsync("DeletedReceipt", roomId, messageId);
    }
}
