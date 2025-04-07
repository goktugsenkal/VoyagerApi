using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SearchRepository(DataContext context) : ISearchRepository
{
    public async Task<List<Voyage>> SearchVoyagesAsync(string query)
    {
        return await context.Voyages
            .Where(v =>
                EF.Functions.ILike(v.Title, $"%{query}%") ||
                EF.Functions.ILike(v.Description, $"%{query}%") ||
                EF.Functions.ILike(v.LocationName, $"%{query}%"))
            .Take(20)
            .ToListAsync();
    }

    public async Task<List<Voyage>> SearchVoyagesByStopMatchAsync(string query)
    {
        return await context.Stops
            .Where(s =>
                EF.Functions.ILike(s.Name, $"%{query}%") ||
                EF.Functions.ILike(s.Description, $"%{query}%"))
            .Select(s => s.Voyage)
            .Distinct()
            .ToListAsync();
    }


    public async Task<List<VoyagerUser>> SearchUsersAsync(string query)
    {
        return await context.Users
            .Where(u =>
                EF.Functions.ILike(u.Username, $"%{query}%") ||
                EF.Functions.ILike(u.FirstName, $"%{query}%") ||
                EF.Functions.ILike(u.LastName, $"%{query}%") ||
                EF.Functions.ILike(u.Bio, $"%{query}%"))
            .Take(20)
            .ToListAsync();
    }
}