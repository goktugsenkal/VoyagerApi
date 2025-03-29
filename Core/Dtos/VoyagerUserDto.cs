using System;

namespace Core.Dtos;

public class VoyagerUserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string BannerPictureUrl { get; set; } = string.Empty;
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public int PlanCount { get; set; }
    public int CompletedPlanCount { get; set; }
    public int LikeCount { get; set; }
    public int InspiredCount { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public bool PhoneNumberConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

