using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserService(
    IVoyageRepository voyageRepository, 
    IUserRepository userRepository, 
    IUserChangeLogRepository logRepository) : IUserService
{
    public async Task<string?> GetUsernameByIdAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id);
        
        // can return null
        return user?.Username;
    }
    
    public async Task<VoyagerUser?> GetUserByIdAsync(Guid id)
    {
        //
        // todo: check if user is allowed to see the other user
        //
            
        var user = await userRepository.GetByIdAsync(id);

        // can return null
        return user;
    }
    
    public async Task<VoyagerUser?> GetUserByEmailAsync(string email)
    {
        return await userRepository.GetByEmailAsync(email);
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
        await userRepository.UpdateAsync(user);
    }

    public async Task UpdateProfileAsync(Guid userId, UpdateProfileModel model, string ipAddress, string userAgent)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException("User not found.");
        
        if (await userRepository.UsernameExistsAsync(model.Username, userId))
            throw new InvalidOperationException("Username is already taken.");
        
        LogIfChanged(userId, user.Username, nameof(user.FirstName), user.FirstName, model.FirstName, ipAddress, userAgent);
        LogIfChanged(userId, user.Username, nameof(user.LastName), user.LastName, model.LastName, ipAddress, userAgent);
        LogIfChanged(userId, user.Username, nameof(user.Bio), user.Bio, model.Bio, ipAddress, userAgent);
        LogIfChanged(userId, user.Username, nameof(user.Username), user.Username, model.Username, ipAddress, userAgent);
            
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Bio = model.Bio;
        user.Username = model.Username;

        await userRepository.UpdateAsync(user);
        await logRepository.SaveChangesAsync();
    }

    public async Task PatchProfileAsync(Guid userId, PatchProfileModel model, string ipAddress, string userAgent)
    {
        var user = await userRepository.GetByIdAsync(userId)
                   ?? throw new KeyNotFoundException("User not found");

        if (model.FirstName is not null && model.FirstName != user.FirstName)
        {
            LogIfChanged(userId, user.Username, nameof(user.FirstName), user.FirstName, model.FirstName, ipAddress, userAgent);
            user.FirstName = model.FirstName;
        }
        
        if (model.LastName is not null && model.LastName != user.LastName)
        {
            LogIfChanged(userId, user.Username, nameof(user.LastName), user.LastName, model.LastName, ipAddress, userAgent);
            user.LastName = model.LastName;
        }

        if (model.Bio is not null && model.Bio != user.Bio)
        {
            LogIfChanged(userId, user.Username, nameof(user.Bio), user.Bio, model.Bio, ipAddress, userAgent);
            user.Bio = model.Bio;
        }

        if (model.Username is not null && model.Username != user.Username)
        {
            if (await userRepository.UsernameExistsAsync(model.Username, userId))
                throw new InvalidOperationException("Username is already taken.");

            LogIfChanged(userId, user.Username, nameof(user.Username), user.Username, model.Username, ipAddress, userAgent);
            user.Username = model.Username;
        }

        await userRepository.UpdateAsync(user);
        await logRepository.SaveChangesAsync();
    }
    
    private void LogIfChanged(Guid userId, string changedByUsername, string fieldName, string? oldValue, string? newValue, string ipAddress, string userAgent)
    {
        if (oldValue != newValue)
        {
            var log = new UserChangeLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ChangedByUsername = changedByUsername,
                FieldName = fieldName,
                OldValue = oldValue,
                NewValue = newValue,
                ChangedAt = DateTime.UtcNow,
                IPAddress = ipAddress,
                UserAgent = userAgent
            };

            logRepository.Add(log); // this should be a non-async method in your repo
        }
    }
}