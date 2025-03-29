using Core.Entities;

namespace Core.Interfaces;

public interface IUserChangeLogRepository
{
    void Add(UserChangeLog log); // sync method, called from within service
    Task SaveChangesAsync();     // commit all logs together
}
