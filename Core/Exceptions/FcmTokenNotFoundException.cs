namespace Core.Exceptions;


public class FcmTokenNotFoundException : Exception
{
    public FcmTokenNotFoundException(Guid userId)
        : base($"FCM token not found for user {userId}")
    {
        
    }
}