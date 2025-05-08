using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(DataContext dataContext) : IUserRepository
{
    public PagedList<VoyagerUser> GetAllAsPagedList(int pageNumber, int pageSize)
    {
        var users = dataContext.Users
            .OrderBy(u => u.CreatedAt)
            .AsQueryable();

        return PagedList<VoyagerUser>.CreatePagedList(users, pageNumber, pageSize);
    }

    public async Task<VoyagerUser?> GetByIdAsync(Guid id)
    {
        return await dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<string?>> GetFcmTokensByIdAsync(Guid id)
    {
        return await dataContext.UserSessions
            .Where(u => u.UserId == id && u.FcmToken != null
                                       && u.RevokedAt == null && u.RevokedByIp == null)
            .Select(u => u.FcmToken)
            .ToListAsync();
    }

    public async Task<VoyagerUser?> GetByEmailAsync(string email)
    {
        return await dataContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<string?> GetUsernameByIdAsync(Guid id)
    {
        var user = await dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        return user?.Username;
    }

    public async Task<bool> UsernameExistsAsync(string username, Guid excludeUserId)
    {
        var normalized = username.ToLower();
        return await dataContext.Users
            .AnyAsync(u => u.Username.ToLower() == normalized && u.Id != excludeUserId);
    }

    public async Task UpdateAsync(VoyagerUser user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        dataContext.Users.Update(user);
        await dataContext.SaveChangesAsync();
    }
}