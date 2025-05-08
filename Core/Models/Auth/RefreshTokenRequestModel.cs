namespace Core.Dtos;

public class RefreshTokenRequestModel
{
    public Guid UserId { get; set; }
    public string? FcmToken { get; set; }
    public required string RefreshToken { get; set; }
    public Guid DeviceId { get; set; }
}