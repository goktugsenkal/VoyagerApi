using Core.Models;

namespace Core.Entities;

public class VoyagerUser : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;

    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public int PlanCount { get; set; }
    public int CompletedPlanCount { get; set; }
    public int LikeCount { get; set; }
    public int InspiredCount { get; set; }
    
    public string Username { get; set; } = string.Empty;
    public string NormalizedUsername { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; } = string.Empty;

    public string PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }

    public int AccessFailedCount { get; set; }
    public bool LockoutEnabled { get; set; }
    public DateTime? LockoutEnd { get; set; }
    
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
