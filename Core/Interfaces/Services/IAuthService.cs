using Core.Dtos;
using Core.Entities;
using Core.Models;

namespace Core.Interfaces.Services;

public interface IAuthService
{
    Task<CheckAvailabilityDto> CheckAvailabilityAsync(CheckAvailabilityModel request);
    Task<VoyagerUser?> RegisterAsync(RegisterModel request);
    Task<TokenResponseDto?> LoginAsync(LoginModel request, string ipAddress);
    Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestModel request, string ipAddress);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordModel model);
    Task SendVerificationEmailAsync(string to, string? username, Guid userId);
    Task<bool> VerifyEmailAsync(Guid userId, string token);
    Task SendPasswordResetEmailAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    string CreateReceiptToken(Guid userId, Guid messageId);
    (Guid UserId, Guid MessageId)? ValidateReceiptToken(string token);
}