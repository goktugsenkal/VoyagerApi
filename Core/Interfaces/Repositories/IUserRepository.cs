using Core.Entities;

namespace Core.Interfaces;

public interface IUserRepository
{
    Task<VoyagerUser?> GetByIdAsync(Guid id);
    Task<VoyagerUser?> GetByEmailAsync(string email);
    Task<string?> GetUsernameByIdAsync(Guid id);
    Task<bool> UsernameExistsAsync(string username, Guid excludeUserId);
    Task UpdateAsync(VoyagerUser user);
}
