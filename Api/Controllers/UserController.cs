using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("users")]
[ApiController]
public class UserController(IAuthService authService, 
    IUserService userService, 
    IVoyageService voyageService) : BaseApiController
{
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<VoyagerUserDto>> Me()
    {
        var voyagerUserId = GetUserIdFromTokenClaims();
        if (voyagerUserId is null)
            return Unauthorized("User ID not found in token claims.");

        var user = await userService.GetUserDtoByIdAsync((Guid)voyagerUserId);
            
        if (user is null)
            return Unauthorized("User not found.");
            
        return Ok(user);
    }
    
    [HttpGet("{userId}")]
    [Authorize]
    public async Task<ActionResult<VoyagerUserDto>> GetUserById(string userId)
    {
        if (!Guid.TryParse(userId, out var voyagerUserId))
        {
            return BadRequest("Invalid user id format.");
        }

        var userDto = await userService.GetUserDtoByIdAsync(voyagerUserId);

        if (userDto is null)
        {
            return Unauthorized("User not found.");
        }

        return Ok(userDto);
    }

    
    [HttpGet("{userId}/voyages")]
    [Authorize]
    public async Task<ActionResult<PagedList<VoyageDto>>> GetVoyagesOfAUser
        (string userId, int pageNumber = 1, int pageSize = 10)
    {
        var voyagerUserId = GetUserIdFromTokenClaims();
        if (voyagerUserId is null)
            return Unauthorized("User ID not found in token claims.");
        
        if (pageSize < 1 || pageNumber < 1 || pageSize > 20)
        {
            return BadRequest("Page number and page size must be greater than 1 and page size must be less than or equal to 20.");
        }
        
        return await userService.GetVoyagesOfAUserAsync(userId, pageNumber, pageSize, voyagerUserId.Value); // value is checked for null
    }
    
    [HttpPut("/profile")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize]
    public async Task<ActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
    {
        var userId = GetUserIdFromTokenClaims();
        if (userId is null)
            return Unauthorized("User ID not found in token claims.");
        
        var ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString()
                 ?? HttpContext.Connection.RemoteIpAddress?.ToString()
                 ?? "unknown";

        var userAgent = Request.Headers.UserAgent.ToString();

        try
        {
            // userId is not null
            await userService.UpdateProfileAsync(userId.Value, model, ip, userAgent);
            return NoContent(); // 204
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Username is already taken"))
        {
            return Conflict(new { error = ex.Message }); // 409
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message }); // 404
        }
    }
    
    [HttpPatch("/profile")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize]
    public async Task<ActionResult> PatchProfile([FromBody] PatchProfileModel model)
    {
        var userId = GetUserIdFromTokenClaims();
        if (userId is null)
            return Unauthorized("User ID not found in token claims.");
        
        var ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString()
                 ?? HttpContext.Connection.RemoteIpAddress?.ToString()
                 ?? "unknown";

        var userAgent = Request.Headers.UserAgent.ToString();

        try
        {
            // userId is not null
            await userService.PatchProfileAsync(userId.Value, model, ip, userAgent);
            return NoContent(); // 204
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Username is already taken"))
        {
            return Conflict(new { error = ex.Message }); // 409
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message }); // 404
        }
    }
    
    [HttpPost("/profile/images")]
    [Authorize]
    public async Task<IActionResult> GetPresignedUploadUrls([FromBody] UserImageUploadModel model)
    {
        var userId = GetUserIdFromTokenClaims();
        if (userId is null)
            return Unauthorized("User ID not found in token claims.");
        
        // userId is not null
        var result = await userService.GeneratePresignedUploadUrlsAsync(userId.Value, model);
        return Ok(result);
    }
    
    [HttpPut("/profile/images")]
    [Authorize]
    public async Task<IActionResult> ConfirmImageUpload([FromBody] UpdateUserImagesModel model)
    {
        var userId = GetUserIdFromTokenClaims();
        if (userId is null)
            return Unauthorized("User ID not found in token claims.");
        
        var ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString()
                 ?? HttpContext.Connection.RemoteIpAddress?.ToString()
                 ?? "unknown";

        var userAgent = Request.Headers.UserAgent.ToString();

        await userService.UpdateProfileImageUrlsAsync(userId.Value, model.ProfilePictureKey, model.BannerPictureKey, ip, userAgent);
        
        return NoContent();
    }
}