using Core.Dtos;
using Core.Entities;
using Core.Models;

namespace Core.Interfaces;

public interface IAuthService
{
    Task<CheckAvailabilityDto> CheckAvailabilityAsync(CheckAvailabilityModel request);
    Task<VoyagerUser?> RegisterAsync(RegisterModel request);
    Task<TokenResponseDto?> LoginAsync(LoginModel request);
    Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestModel request);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordModel model);
    Task SendVerificationEmailAsync(string to, string? username, Guid userId);
    Task<bool> VerifyEmailAsync(Guid userId, string token);
    Task SendPasswordResetEmailAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
}