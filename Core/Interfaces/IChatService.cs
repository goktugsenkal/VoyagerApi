namespace Core.Interfaces;

public interface IChatService
{
    Task SaveMessageAsync(Guid userId, string message);
}
