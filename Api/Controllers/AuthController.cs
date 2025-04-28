using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController(IAuthService authService, IUserService userService, IEmailService emailService) : BaseApiController
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
                return Conflict("Username or email already exists.");
            }

            // Map the domain model to a DTO to avoid exposing sensitive details
            var userDto = user.ToDto();

            return Ok(userDto);
        }


        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginModel request)
        {
            try
            {
                var result = await authService.LoginAsync(request);
                if (result is null)
                    return BadRequest("Invalid username or password.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);   
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestModel request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result?.AccessToken is null)
                return Unauthorized("Invalid refresh token.");

            return Ok(result);
        }
        
        [HttpPost("request-email-verification")]
        public async Task<IActionResult> RequestEmailVerification()
        {
            var userToken = GetUserIdFromTokenClaims();
            if (userToken == null)
                return Unauthorized("User ID not found in token claims.");

            var user = await userService.GetUserByIdAsync((Guid)userToken);
            if (user == null)
                return Unauthorized("User not found.");

            await authService.SendVerificationEmailAsync(user.Email, user.Username, user.Id);
            return Ok("Verification email sent.");
        }

        [AllowAnonymous]
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] Guid userId, [FromQuery] string token)
        {
            // Call the high-level service method.
            var isVerified = await authService.VerifyEmailAsync(userId, token);
            if (!isVerified)
                return BadRequest("Invalid or expired token.");

            return Ok("Email verified successfully.");
        }

        
        [Authorize]
        [HttpPost("password/change")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var userId = GetUserIdFromTokenClaims();
            if (userId == null)
                return Unauthorized("User ID not found in token claims.");

            try
            {
                await authService.ChangePasswordAsync(userId.Value, model);
                return Ok("Password changed successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("An unexpected error occurred while changing the password.");
            }
        }

        [HttpPost("password/forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            // Call the service layer to send the password reset email.
            // The service method encapsulates token generation and email sending.
            await authService.SendPasswordResetEmailAsync(request.Email);
        
            // Always return a generic response to avoid leaking account existence information.
            return Ok(new 
            { 
                Message = "If an account with that email exists, you will receive a password reset email shortly." 
            });
        }
        

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel request)
        {
            // The ResetPasswordAsync service method internally validates the token.
            var result = await authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
            if (!result)
                return BadRequest("Invalid token or email.");

            return Ok("Password has been reset successfully.");
        }
    }
}
