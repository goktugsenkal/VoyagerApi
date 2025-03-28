using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserService(IVoyageRepository voyageRepository, DataContext context) : IUserService
{
    public async Task<string?> GetUsernameByIdAsync(Guid id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        return user?.Username;
    }
    
    public async Task<VoyagerUser?> GetUserByEmailAsync(string email)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user;
    }

    public async Task<PagedList<VoyageDto>> GetVoyagesOfAUserAsync(string userId, int pageNumber, int pageSize)
    {
        Guid voyagerUserId;
        try
        {
            voyagerUserId = Guid.Parse(userId);
        }
        catch (Exception e)
        {
            throw new Exception("Invalid user ID format.");
        }
        
        var voyages = await voyageRepository.GetVoyagesByVoyagerUserIdAsync(voyagerUserId, pageNumber, pageSize);
        
        // map voyages to VoyageDtos
        // and map stops to StopDtos
        var voyageDtos = voyages.Items.Select(voyage => voyage.ToDto()).ToList();

        return new PagedList<VoyageDto>(voyageDtos, voyages.TotalCount, voyages.CurrentPage, voyages.PageSize);
    }

    public async Task UpdateUserAsync(VoyagerUser user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task<VoyagerUser?> GetUserByIdAsync(Guid id)
    {
        //
        // todo: check if user is allowed to see the other user
        //
            
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
        {
            return null;
        }

        return user;
    }
}