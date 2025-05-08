namespace Core.Interfaces.Services;

public interface IFcmService
{
    Task<string> SendNotificationAsync(string fcmToken, string title, string body, IDictionary<string,string>? data = null);
    Task<string> SendBackgroundNotificationAsync(string fcmToken, IDictionary<string,string>? data = null);
}