namespace Core.Models;

public class LoginModel
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required Guid DeviceId { get; set; }
}