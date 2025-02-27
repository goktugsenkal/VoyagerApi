using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : BaseApiController
    {
        [HttpPost("check-availability")]
        public async Task<ActionResult<CheckAvailabilityDto>> CheckAvailability(CheckAvailabilityModel request)
        {
            var availabilityDto = await authService.CheckAvailabilityAsync(request);
            return Ok(availabilityDto);
        }
        
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

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token.");

            return Ok(result);
        }
    }
}
