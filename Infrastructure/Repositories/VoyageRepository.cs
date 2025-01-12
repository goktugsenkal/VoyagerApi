using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VoyageRepository(DataContext dataContext, IStopRepository stopRepository) : IVoyageRepository
{
    public async Task<ICollection<Voyage>> GetAllVoyages()
    {
        throw new NotImplementedException();
    }

    public async Task<Voyage?> GetVoyageById(Guid voyageId)
    {
        return await dataContext.Voyages.FirstOrDefaultAsync(v => v.Id == voyageId);
    }

    public async Task AddAsync(Voyage voyage)
    {
        voyage.CreatedAt = DateTime.UtcNow;
        await dataContext.Voyages.AddAsync(voyage);
        await dataContext.SaveChangesAsync();
    }


    public async Task UpdateVoyage(Voyage voyage)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteVoyage(Voyage voyage)
    {
        dataContext.Voyages.Remove(voyage);
        await dataContext.SaveChangesAsync();
    }
}