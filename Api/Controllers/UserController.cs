using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("users")]
[ApiController]
public class UserController(IAuthService authService, IUserService userService) : BaseApiController
{
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<VoyagerUserDto>> Me()
    {
        var voyagerUserId = GetUserIdFromTokenClaims();
        if (voyagerUserId is null)
            return Unauthorized("User ID not found in token claims.");
            
        var user = await userService.GetUserByIdAsync((Guid)voyagerUserId);
            
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

        var user = await userService.GetUserByIdAsync(voyagerUserId);

        if (user is null)
        {
            return Unauthorized("User not found.");
        }

        return Ok(user);
    }

    
    [HttpGet("{userId}/voyages")]
    [Authorize]
    public async Task<ActionResult<PagedList<VoyageDto>>> GetVoyagesOfAUser
        (string userId, int pageNumber = 1, int pageSize = 10)
    {
        if (pageSize < 1 || pageNumber < 1 || pageSize > 20)
        {
            return BadRequest("Page number and page size must be greater than 1 and page size must be less than or equal to 20.");
        }
        
        return await userService.GetVoyagesOfAUserAsync(userId, pageNumber, pageSize);
    }
}