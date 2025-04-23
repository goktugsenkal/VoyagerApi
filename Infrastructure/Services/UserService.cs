using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Core.Models;

namespace Infrastructure.Services;

public class UserService(
    IVoyageRepository voyageRepository, 
    IUserRepository userRepository,
    ILikeRepository likeRepository,
    IUserChangeLogRepository logRepository, 
    IS3Service s3Service) : IUserService
{
    public async Task<string?> GetUsernameByIdAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id);
        
        // can return null
        return user?.Username;
    }

    public async Task<PagedList<VoyagerUserDto>> GetAllUsersAsync(int pageNumber, int pageSize)
    {
        var users = userRepository.GetAllAsPagedList(pageNumber, pageSize);

        var userDtos = users.Items.Select(user => 
        {
            var userDto = user.ToDto();

            if (!string.IsNullOrWhiteSpace(user.ProfilePictureUrl))
                userDto.ProfilePictureUrl = s3Service.GeneratePreSignedDownloadUrl(user.ProfilePictureUrl, TimeSpan.FromMinutes(15));
            if (!string.IsNullOrWhiteSpace(user.BannerPictureUrl))
                userDto.BannerPictureUrl = s3Service.GeneratePreSignedDownloadUrl(user.BannerPictureUrl, TimeSpan.FromMinutes(15));

            return userDto;
        }).ToList();

        return new PagedList<VoyagerUserDto>(userDtos, users.TotalCount, users.CurrentPage, users.PageSize);
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

    public async Task<VoyagerUserDto?> GetUserDtoByIdAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user == null)
            return null;

        var userDto = user.ToDto();

        if (!string.IsNullOrWhiteSpace(user.ProfilePictureUrl))
            userDto.ProfilePictureUrl = s3Service.GeneratePreSignedDownloadUrl(user.ProfilePictureUrl, TimeSpan.FromMinutes(15));

        if (!string.IsNullOrWhiteSpace(user.BannerPictureUrl))
            userDto.BannerPictureUrl = s3Service.GeneratePreSignedDownloadUrl(user.BannerPictureUrl, TimeSpan.FromMinutes(15));

        return userDto;
    }

    public async Task<VoyagerUser?> GetUserByEmailAsync(string email)
    {
        return await userRepository.GetByEmailAsync(email);
    }

    public async Task<PagedList<VoyageDto>> GetVoyagesOfAUserAsync(string userId, 
        int pageNumber, 
        int pageSize,
        Guid consumerUserId)
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
        
        
        
        // convert image keys to s3 presigned download urls
        foreach (var voyageDto in voyageDtos)
        {
            var voyagerUser = await userRepository.GetByIdAsync(voyageDto.VoyagerUserId);
            
            voyageDto.VoyagerUsername = voyagerUser?.Username ?? "Voyager User";
            voyageDto.ProfilePictureUrl = s3Service.GeneratePreSignedDownloadUrl(voyagerUser?.ProfilePictureUrl ?? "", TimeSpan.FromMinutes(10));
            voyageDto.IsLiked = await likeRepository.ExistsAsync(voyageDto.Id, null, consumerUserId);

            if (voyageDto.MediaUrls != null && voyageDto.MediaUrls.Any())
            {
                voyageDto.MediaUrls = voyageDto.MediaUrls
                    .Select(key => s3Service.GeneratePreSignedDownloadUrl(key, TimeSpan.FromMinutes(15)))
                    .ToList();
            }
    
            if (voyageDto.Stops != null && voyageDto.Stops.Any())
            {
                foreach (var stopDto in voyageDto.Stops)
                {
                    if (stopDto.MediaUrls != null && stopDto.MediaUrls.Any())
                    {
                        stopDto.MediaUrls = stopDto.MediaUrls
                            .Select(key => s3Service.GeneratePreSignedDownloadUrl(key, TimeSpan.FromMinutes(15)))
                            .ToList();
                    }
                }
            }
        }

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
        LogIfChanged(userId, user.Username, nameof(user.Location), user.Location, model.Location, ipAddress, userAgent);
        LogIfChanged(userId, user.Username, nameof(user.Username), user.Username, model.Username, ipAddress, userAgent);
            
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Bio = model.Bio;
        user.Location = model.Location;
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
        
        if (model.Location is not null && model.Location != user.Location)
        {
            LogIfChanged(userId, user.Username, nameof(user.Location ), user.Location , model.Location , ipAddress, userAgent);
            user.Location = model.Location;
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

    public async Task<UserImageUploadDto> GeneratePresignedUploadUrlsAsync(Guid userId, UserImageUploadModel request)
    {
        var response = new UserImageUploadDto();

        if (request.UpdateProfilePicture)
        {
            var profileImageId = Guid.NewGuid().ToString("N");
            var profileKey = $"users/{userId}/{profileImageId}.jpg";

            response.ProfilePictureUploadUrl = s3Service.GeneratePreSignedUploadUrl(profileKey, TimeSpan.FromMinutes(10));
            response.ProfilePictureUrl = profileKey;
        }

        if (request.UpdateBannerPicture)
        {
            var bannerImageId = Guid.NewGuid().ToString("N");
            var bannerKey = $"users/{userId}/{bannerImageId}.jpg";

            response.BannerPictureUploadUrl = s3Service.GeneratePreSignedUploadUrl(bannerKey, TimeSpan.FromMinutes(10));
            response.BannerPictureUrl = bannerKey;
        }

        return response;
    }

    public async Task UpdateProfileImageUrlsAsync(Guid userId, string? profilePictureKey, string? bannerPictureKey, string ipAddress, string userAgent)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found");

        if (!string.IsNullOrWhiteSpace(profilePictureKey))
        {
            LogIfChanged(userId, user.Username, nameof(user.ProfilePictureUrl), user.ProfilePictureUrl, profilePictureKey, ipAddress, userAgent);
            user.ProfilePictureUrl = profilePictureKey; // just the key
        }

        if (!string.IsNullOrWhiteSpace(bannerPictureKey))
        {
            LogIfChanged(userId, user.Username, nameof(user.BannerPictureUrl), user.BannerPictureUrl, bannerPictureKey, ipAddress, userAgent);
            user.BannerPictureUrl = bannerPictureKey; // just the key
        }

        await userRepository.UpdateAsync(user);
    }




    private void LogIfChanged(Guid userId, string changedByUsername, string fieldName, string? oldValue, string? newValue, string ipAddress, string userAgent)
    {
        if (oldValue != newValue)
        {
            var log = new UserChangeLog
            {
                Id = Guid.NewGuid(),
                VoyagerUserId = userId,
                ChangedByUsername = changedByUsername,
                FieldName = fieldName,
                OldValue = oldValue,
                NewValue = newValue,
                ChangedAt = DateTime.UtcNow,
                IPAddress = ipAddress,
                UserAgent = userAgent
            };

            logRepository.Add(log);
        }
    }
}