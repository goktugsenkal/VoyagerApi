using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VoyageRepository(DataContext dataContext, IStopRepository stopRepository) : IVoyageRepository
{
    public async Task<ICollection<Voyage>> GetAllVoyages()
    {
        // for now, return ALL voyages in the database
        // at the 1000 user mark, I will rewrite this
        
        // todo: add pagination
        return await dataContext.Voyages.ToListAsync();
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
        dataContext.Voyages.Update(voyage);
        await dataContext.SaveChangesAsync();
    }

    public async Task DeleteVoyage(Voyage voyage)
    {
        dataContext.Voyages.Remove(voyage);
        await dataContext.SaveChangesAsync();
    }

    public async Task<bool> IncrementLikesAsync(Guid voyageId)
    {
        var voyage = await dataContext.Voyages.FirstOrDefaultAsync(v => v.Id == voyageId);
        
        if (voyage is null)
        {
            return false;
        }
        
        voyage.LikeCount++;
        await dataContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DecrementLikesAsync(Guid voyageId)
    {
        var voyage = await dataContext.Voyages.FirstOrDefaultAsync(v => v.Id == voyageId);
        
        if (voyage is null)
        {
            return false;
        }
        
        voyage.LikeCount--;
        await dataContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IncrementCommentsAsync(Guid voyageId)
    {
        var voyage = await GetVoyageById(voyageId);
        
        if (voyage is null)
        {
            return false;
        }
        
        voyage.CommentCount++;
        await dataContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DecrementCommentsAsync(Guid voyageId)
    {
        var voyage = await GetVoyageById(voyageId);
        
        if (voyage is null)
        {
            return false;
        }
        
        voyage.CommentCount--;
        await dataContext.SaveChangesAsync();
        return true;
    }
}