using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class VoyageRepository(DataContext dataContext) : IVoyageRepository
{
    public async Task<ICollection<Voyage>> GetAllVoyages()
    {
        throw new NotImplementedException();
    }

    public async Task<Voyage> GetVoyageById(Guid voyageId)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }
}