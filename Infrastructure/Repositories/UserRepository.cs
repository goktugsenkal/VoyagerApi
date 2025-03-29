using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(DataContext dataContext) : IUserRepository
{
    public async Task<VoyagerUser?> GetByIdAsync(Guid id)
    {
        return await dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<VoyagerUser?> GetByEmailAsync(string email)
    {
        return await dataContext.Users.FirstOrDefaultAsync(u => u.Email == email);
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