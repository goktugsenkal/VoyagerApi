using Core.Dtos;
using Core.Models;

namespace Core.Entities;

public class VoyagerUser : UpdatableBaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayName => $"{FirstName} {LastName}".Trim();
    public string Bio { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty; //
    
    public string ProfilePictureUrl { get; set; } = string.Empty; //
    public string BannerPictureUrl { get; set; } = string.Empty; //

    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public int PlanCount { get; set; }
    public int CompletedPlanCount { get; set; }
    public int LikeCount { get; set; }
    public int InspiredCount { get; set; }
    
    public string Username { get; set; } = string.Empty; //
    public string NormalizedUsername { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty; //
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
    public bool PhoneNumberConfirmed { get; set; }

    public int AccessFailedCount { get; set; }
    public bool LockoutEnabled { get; set; }
    public DateTime? LockoutEnd { get; set; }
    
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public ICollection<Voyage> Voyages { get; set; }
}

public static class VoyagerUserExtensions
{
    public static VoyagerUserDto ToDto(this VoyagerUser user)
    {
        return new VoyagerUserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            Location = user.Location,
            ProfilePictureUrl = user.ProfilePictureUrl,
            BannerPictureUrl = user.BannerPictureUrl,
            FollowerCount = user.FollowerCount,
            FollowingCount = user.FollowingCount,
            PlanCount = user.PlanCount,
            CompletedPlanCount = user.CompletedPlanCount,
            LikeCount = user.LikeCount,
            InspiredCount = user.InspiredCount,
            Username = user.Username,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
