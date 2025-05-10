namespace Core.Exceptions;

public class ChatRoomNotFoundException : Exception
{
    public ChatRoomNotFoundException(Guid roomId) : base($"Chat room {roomId} not found") { } 
}