using Core.Dtos;
using Core.Entities;
using Core.Models;

namespace Core.Interfaces;

public interface IAuthService
{
    Task<VoyagerUser?> RegisterAsync(RegisterModel request);
    Task<TokenResponseDto?> LoginAsync(LoginModel request);
    Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
}