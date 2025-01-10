using Core.Dtos;
using Core.Models;

namespace Core.Interfaces;

public interface IAuthService
{
    Task<VoyagerUser?> RegisterAsync(VoyagerUserDto request);
    Task<TokenResponseDto?> LoginAsync(VoyagerUserDto request);
    Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
}