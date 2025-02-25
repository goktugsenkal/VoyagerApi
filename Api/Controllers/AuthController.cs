using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<VoyagerUser>> Register(RegisterModel request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
                return Conflict("Username already exists.");

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginModel request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
                return BadRequest("Invalid username or password.");

            return Ok(result);
        }
        
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

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token.");

            return Ok(result);
        }
        
        [HttpGet("test")]
        [AllowAnonymous]
        public ActionResult<string> Get()
        {
            return Ok("Hello from the API! CI&CD Testing.");
        }
        
    }
}
