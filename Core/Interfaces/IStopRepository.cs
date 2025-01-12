using Core.Entities;

namespace Core.Interfaces;

public interface IStopRepository
{
    Task AddRangeAsync(ICollection<Stop> stops);
}