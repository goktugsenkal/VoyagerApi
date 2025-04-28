namespace Core.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Guid DeviceId { get; set; }
    public string CreatedByIp { get; set; }
    
    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    
    public Guid UserId { get; set; }
    public VoyagerUser User { get; set; }
}