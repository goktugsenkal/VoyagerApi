using Core.Dtos;
using Core.Models;

namespace Core.Interfaces;

public interface IUserService
{
    Task<VoyagerUserDto?> GetUserByIdAsync(Guid id);
    Task<string?> GetUserNameByIdAsync(Guid id);
    Task<PagedList<VoyageDto>> GetVoyagesOfAUserAsync(string userId,  int pageNumber, int pageSize);
}