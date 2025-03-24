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
        [HttpPost("check-availability")]
        public async Task<ActionResult<CheckAvailabilityDto>> CheckAvailability(CheckAvailabilityModel request)
        {
            var availabilityDto = await authService.CheckAvailabilityAsync(request);
            return Ok(availabilityDto);
        }
        
        [HttpPost("register")]
        public async Task<ActionResult<VoyagerUserDto>> Register([FromBody] RegisterModel request)
        {
            // Validate incoming model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Attempt to register the user
            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                return Conflict("Username already exists.");
            }

            // Map the domain model to a DTO to avoid exposing sensitive details
            var userDto = user.ToDto();

            return Ok(userDto);
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
            if (result?.AccessToken is null)
                return Unauthorized("Invalid refresh token.");

            return Ok(result);
        }
        
        [HttpPost("request-email-verification")]
        public async Task<IActionResult> RequestEmailVerification()
        {
            throw new NotImplementedException();
        }

        private async Task SendVerificationEmail(string email, string userName, string verificationLink)
        {
            throw new NotImplementedException();
        }

        [AllowAnonymous]
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            throw new NotImplementedException();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            throw new NotImplementedException();
        }

        private async Task SendResetPasswordEmailAsync()
        {
            throw new NotImplementedException();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string email)
        {
            throw new NotImplementedException();
        }
    }
}
