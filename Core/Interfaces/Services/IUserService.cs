using Core.Dtos;
using Core.Entities;
using Core.Models;

namespace Core.Interfaces;

public interface IUserService
{
    Task<VoyagerUser?> GetUserByIdAsync(Guid id);
    Task<VoyagerUserDto?> GetUserDtoByIdAsync(Guid id);
    Task<VoyagerUser?> GetUserByEmailAsync(string email);
    Task<string?> GetUsernameByIdAsync(Guid id);
    Task<PagedList<VoyageDto>> GetVoyagesOfAUserAsync(string userId, int pageNumber, int pageSize, Guid consumerUserId);
    Task UpdateUserAsync(VoyagerUser user);
    Task UpdateProfileAsync(Guid userId, UpdateProfileModel model, string ipAddress, string userAgent);
    Task PatchProfileAsync(Guid userId, PatchProfileModel model, string ipAddress, string userAgent);
    Task<UserImageUploadDto> GeneratePresignedUploadUrlsAsync(Guid userId, UserImageUploadModel request);
    Task UpdateProfileImageUrlsAsync(Guid userId, string? profileUrl, string? bannerUrl, string ipAddress, string userAgent);
}