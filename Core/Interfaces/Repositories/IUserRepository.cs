using Core.Entities;
using Core.Models;

namespace Core.Interfaces.Repositories;

public interface IUserRepository
{
    PagedList<VoyagerUser> GetAllAsPagedList(int pageNumber, int pageSize);
    Task<VoyagerUser?> GetByIdAsync(Guid id);
    Task<List<string?>> GetFcmTokensByIdAsync(Guid id);
    Task<VoyagerUser?> GetByEmailAsync(string email);
    Task<string?> GetUsernameByIdAsync(Guid id);
    Task<bool> UsernameExistsAsync(string username, Guid excludeUserId);
    Task UpdateAsync(VoyagerUser user);
}
