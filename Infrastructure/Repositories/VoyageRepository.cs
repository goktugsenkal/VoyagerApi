using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VoyageRepository(DataContext dataContext) : IVoyageRepository
// (IStopRepository stopRepository) was being injected in here. idk why.
// add it back if necessary.
{
    public async Task<ICollection<Voyage>> GetAllAsync()
    {
        // for now, return ALL voyages in the database
        
        // todo: add pagination
        return await dataContext.Voyages.ToListAsync();
        
        // at the 1000 user mark, I will rewrite this
    }

    public async Task<Voyage?> GetByIdAsync(Guid voyageId)
    {
        // simple return from Voyages table where Id = voyageId
        return await dataContext.Voyages
            .Include(v => v.Stops)
            .Include(v => v.Comments)
            .Include(v => v.Likes)
            .FirstOrDefaultAsync(v => v.Id == voyageId);
        // or null if there is no Voyage with the specified voyageId.
    }

    public async Task AddAsync(Voyage voyage)
    {
        voyage.CreatedAt = DateTime.UtcNow;
        await dataContext.Voyages.AddAsync(voyage);
        await dataContext.SaveChangesAsync();
    }


    public async Task UpdateAsync(Voyage voyage)
    {
        dataContext.Voyages.Update(voyage);
        await dataContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Voyage voyage)
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
        var voyage = await GetByIdAsync(voyageId);
        
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
        var voyage = await GetByIdAsync(voyageId);
        
        if (voyage is null)
        {
            return false;
        }
        
        voyage.CommentCount--;
        await dataContext.SaveChangesAsync();
        return true;
    }
}