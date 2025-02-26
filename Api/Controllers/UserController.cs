using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("users")]
[ApiController]
public class UserController(IAuthService authService) : BaseApiController
{
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<VoyagerUserDto>> Me()
    {
        var voyagerUserId = GetUserIdFromTokenClaims();
        if (voyagerUserId is null)
            return Unauthorized("User ID not found in token claims.");
            
        var user = await authService.GetUserByIdAsync((Guid)voyagerUserId);
            
        if (user is null)
            return Unauthorized("User not found.");
            
        return Ok(user);
    }
}