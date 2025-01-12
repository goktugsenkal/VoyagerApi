using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class StopRepository(DataContext dataContext) : IStopRepository
{
    public async Task AddRangeAsync(ICollection<Stop> stops)
    {
        foreach (var stop in stops)
        {
            stop.CreatedAt = DateTime.UtcNow;
        }
        
        await dataContext.Stops.AddRangeAsync(stops);
        await dataContext.SaveChangesAsync();
    }
}
