using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VoyageRepository(DataContext dataContext) : IVoyageRepository
// (IStopRepository stopRepository) was being injected in here. idk why.
// add it back if necessary.
{
    public PagedList<Voyage> GetAllAsPagedList(int pageNumber, int pageSize)
    {
        // get voyages DbSet as a queryable
        var voyages = dataContext.Voyages
            // include related entities
            .Include(v => v.Stops)
            .Include(v => v.Comments)
            .Include(v => v.Likes)
            .OrderByDescending(v => v.CreatedAt)
            .AsQueryable();

        // create a PagedList and return it
        return PagedList<Voyage>.CreatePagedList(voyages, pageNumber, pageSize);
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

    /// <summary>
    /// shitty overhead method
    /// </summary>
    /// <param name="voyageId"></param>
    /// <returns>Username</returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> GetUsernameByVoyageIdAsync(Guid voyageId)
    {
        // retrieve the voyage record using the provided voyageId.
        var voyage = await dataContext.Voyages
            .FirstOrDefaultAsync(v => v.Id == voyageId);
    
        if (voyage == null)
        {
            // handle the case when the voyage doesn't exist.
            throw new Exception("Voyage not found.");
        }
    
        // use the VoyagerUserId to retrieve the associated user.
        var user = await dataContext.Users
            .FirstOrDefaultAsync(u => u.Id == voyage.VoyagerUserId);
    
        if (user == null)
        {
            // handle the case when the user is not found.
            throw new Exception("User not found for the given voyage.");
        }
    
        // return the username.
        return user.Username;
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