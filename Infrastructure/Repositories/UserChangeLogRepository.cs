using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class UserChangeLogRepository(DataContext dataContext) : IUserChangeLogRepository
{
    public void Add(UserChangeLog log)
    {
        dataContext.UserChangeLogs.Add(log);
    }

    public async Task SaveChangesAsync()
    {
        await dataContext.SaveChangesAsync();
    }
}