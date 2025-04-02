namespace Core.Dtos;

public class RefreshTokenRequestModel
{
    public Guid UserId { get; set; }
    public required string RefreshToken { get; set; }
}