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
    public async Task SaveChangesAsync()
    {
        await dataContext.SaveChangesAsync();
    }

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
        
        // sort stops
        foreach (var voyage in voyages)
        {
            voyage.SortStops();
        }

        // create a PagedList and return it
        return PagedList<Voyage>.CreatePagedList(voyages, pageNumber, pageSize);
    }
    
    public PagedList<Voyage> GetVoyagesFiltered(double? latitudeMin, double? latitudeMax, double? longitudeMin, double? longitudeMax, int pageNumber, int pageSize)
    {
        var query = dataContext.Voyages.Include(v => v.Stops).AsQueryable();

        // Apply filtering if any coordinate filter is provided.
        if (latitudeMin.HasValue || latitudeMax.HasValue || longitudeMin.HasValue || longitudeMax.HasValue)
        {
            query = query.Where(v => v.Stops.Any(s =>
                s.IsFocalPoint &&
                (!latitudeMin.HasValue || s.Latitude >= latitudeMin.Value) &&
                (!latitudeMax.HasValue || s.Latitude <= latitudeMax.Value) &&
                (!longitudeMin.HasValue || s.Longitude >= longitudeMin.Value) &&
                (!longitudeMax.HasValue || s.Longitude <= longitudeMax.Value)
            ));
        }

        // Apply pagination.
        var voyages = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        foreach (var voyage in voyages)
        {
            voyage.SortStops();
        }

        return new PagedList<Voyage>(voyages, query.Count(), pageNumber, pageSize);
    }

    public async Task<Voyage?> GetByIdAsync(Guid voyageId)
    {
        var voyage = await dataContext.Voyages
            .Include(v => v.Stops)
            .Include(v => v.Comments)
            .Include(v => v.Likes)
            .FirstOrDefaultAsync(v => v.Id == voyageId);

        voyage?.SortStops();

        return voyage;
    }

    public async Task<PagedList<Voyage>> GetVoyagesByVoyagerUserIdAsync(Guid voyagerUserId, int pageNumber, int pageSize)
    {
        var voyagesQuery = dataContext.Voyages
            .Where(v => v.VoyagerUserId == voyagerUserId)
            .Include(v => v.Stops)
            .Include(v => v.Comments)
            .Include(v => v.Likes)
            .OrderByDescending(v => v.CreatedAt);

        var pagedVoyages = PagedList<Voyage>.CreatePagedList(voyagesQuery, pageNumber, pageSize);

        foreach (var voyage in pagedVoyages.Items.Where(v => v.Stops != null && v.Stops.Any()))
        {
            voyage.SortStops();
        }

        return pagedVoyages;
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
        voyage.UpdatedAt = DateTime.UtcNow;
        await dataContext.Voyages.AddAsync(voyage);
        await dataContext.SaveChangesAsync();
    }


    public async Task UpdateAsync(Voyage voyage)
    {
        voyage.UpdatedAt = DateTime.UtcNow;
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