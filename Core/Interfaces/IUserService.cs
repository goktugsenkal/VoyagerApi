using Core.Dtos;
using Core.Entities;
using Core.Models;

namespace Core.Interfaces;

public interface IUserService
{
    Task<VoyagerUser?> GetUserByIdAsync(Guid id);
    Task<VoyagerUser?> GetUserByEmailAsync(string email);
    Task<string?> GetUsernameByIdAsync(Guid id);
    Task<PagedList<VoyageDto>> GetVoyagesOfAUserAsync(string userId,  int pageNumber, int pageSize);
    Task UpdateUserAsync(VoyagerUser user);
    Task UpdateProfileAsync(Guid userId, UpdateProfileModel model, string ipAddress, string userAgent);
    Task PatchProfileAsync(Guid userId, PatchProfileModel model, string ipAddress, string userAgent);

}