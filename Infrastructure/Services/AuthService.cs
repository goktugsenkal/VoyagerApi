using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class AuthService(
    DataContext context, 
    IConfiguration configuration, 
    IUserService userService, 
    IPasswordHasher<VoyagerUser> passwordHasher, 
    IEmailService emailService) : IAuthService
    {
        private readonly byte[] _secretKey = Encoding.UTF8.GetBytes(configuration["SecretKey"]);

        public async Task<TokenResponseDto?> LoginAsync(LoginModel request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user is null)
            {
                return null;
            }
            
            if (user.LockoutEnabled && user.LockoutEnd > DateTime.UtcNow)
                throw new Exception("Account is locked. Try again later.");

            if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                user.AccessFailedCount += 1;
                if (user.AccessFailedCount >= 200) // todo: drop this to ~5 for prod 
                {
                    user.LockoutEnabled = true;
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(10);
                }

                await context.SaveChangesAsync();
                return null;
            }
            
            // Successful login
            user.AccessFailedCount = 0;
            user.LockoutEnabled = false;
            user.LockoutEnd = null;
                
            await context.SaveChangesAsync();
                
            return await CreateTokenResponse(user);
        }

        private async Task<TokenResponseDto> CreateTokenResponse(VoyagerUser? user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        public async Task<CheckAvailabilityDto> CheckAvailabilityAsync(CheckAvailabilityModel request)
        {
            var badWordsService = new BadWordsService();
            
            var emailUnavailable = await context.Users.AnyAsync(u => u.Email == request.Email);
            var usernameUnavailable = await context.Users.AnyAsync(u => u.Username == request.Username)
                || badWordsService.ContainsBadWord(request.Username);
            
            
            var response = new CheckAvailabilityDto
            {
                EmailAvailable = !emailUnavailable,
                UsernameAvailable = !usernameUnavailable
            };

            return response;
        }

        public async Task<VoyagerUser?> RegisterAsync(RegisterModel request)
        {
            if (await context.Users.AnyAsync(u => u.Username == request.Username) ||
                await context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return null;
            }

            var user = new VoyagerUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Username,
                NormalizedUsername = request.Username.Normalize().ToUpper(),
                Email = request.Email,
                NormalizedEmail = request.Email.Normalize().ToUpper(),
                Bio = request.Bio,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };
            var hashedPassword = passwordHasher.HashPassword(user, request.Password);

            user.Username = request.Username;
            user.PasswordHash = hashedPassword;

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestModel request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null)
                return null;

            return await CreateTokenResponse(user);
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordModel model)
        {
            // todo: add a TokenVersion or PasswordChangedAt field to the users table to invalidate old tokens
            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var passwordVerification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword);
            if (passwordVerification == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Current password is incorrect.");

            user.PasswordHash = passwordHasher.HashPassword(user, model.NewPassword);
            await userService.UpdateUserAsync(user); // persist change

            return true;
        }



        private async Task<VoyagerUser?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await context.Users.FindAsync(userId);
            if (user is null || user.RefreshToken != refreshToken
                || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }


        private async Task<string> GenerateAndSaveRefreshTokenAsync(VoyagerUser user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refreshToken;
        }

        private string CreateToken(VoyagerUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task SendVerificationEmailAsync(string to, string? username, Guid userId)
        {
            var token = GenerateAccountActionToken(userId, "VerifyEmail");
            var encodedToken = WebUtility.UrlEncode(token);
            var link = $"https://voyagerapi.com.tr/api/auth/verify-email?userId={userId}&token={encodedToken}";

            var body = $"""
                <p>Hello {username ?? "Voyager User"},</p>
                <p>Please verify your email by clicking the link below:</p>
                <p><a href='{link}'>Verify Email</a></p>
                <p>If you did not request this, please ignore this email.</p>
            """;

            var emailModel = new EmailModel
            {
                To = to,
                From = "noreply@voyagerapi.com.tr",
                Subject = "Verify Your Email Address",
                Body = body
            };

            await emailService.SendEmailAsync(emailModel);
        }
        
        public async Task<bool> VerifyEmailAsync(Guid userId, string token)
        {
            // The internal token validation is hidden.
            if (!ValidateAccountActionToken(token, "VerifyEmail", out Guid tokenUserId) || tokenUserId != userId)
                return false;

            var user = await userService.GetUserByIdAsync(userId);
            if (user == null)
                return false;

            user.EmailConfirmed = true;
            await userService.UpdateUserAsync(user);
            return true;
        }

        
        public async Task SendPasswordResetEmailAsync(string email)
    {
        var user = await userService.GetUserByEmailAsync(email);
        if (user == null)
        {
            // avoid leaking information: if user doesn't exist, do nothing.
            return;
        }
        
        // generate a generic account action token for resetting the password.
        var token = GenerateAccountActionToken(user.Id, "ResetPassword");
        token = WebUtility.UrlEncode(token);
        
        
        // todo: configure for deeplink for Flutter app
        var frontendUrl = configuration["Flutter:ResetPasswordUrl"];
        var resetLink = $"https://voyagerapi.com.tr/reset-password?email={email}&token={token}";
        
        var emailModel = new EmailModel
        {
            To = email,
            From = "noreply@voyagerapi.com.tr",
            Subject = "Reset Your Password",
            Body = $"""
                    
                                <html>
                                  <body style="font-family: Arial, sans-serif; font-size: 14px; color: #333;">
                                    <p>Dear User,</p>
                                    <p>We received a request to reset your password. Please click the link below to reset your password:</p>
                                    <p>
                                      {resetLink}
                                    </p>
                                    <p>This link will expire in 24 hours. If you did not request a password reset, please ignore this email.</p>
                                    <br/>
                                    <p>Best regards,</p>
                                    <p>The Voyager Team</p>
                                  </body>
                                </html>
                    """
        };
        Console.WriteLine(resetLink);
        
        await emailService.SendEmailAsync(emailModel);
    }
    
    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await userService.GetUserByEmailAsync(email);
        if (user == null)
        {
            return false;
        }
        
        // the browser does this automatically, so it's not needed
        // token = WebUtility.UrlDecode(token);
        
        if (!ValidateAccountActionToken(token, "ResetPassword", out Guid tokenUserId))
        {
            return false;
        }
        
        if (user.Id != tokenUserId)
        {
            return false;
        }
        
        user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
        await userService.UpdateUserAsync(user);
        return true;
    }
    
    #region Account Action Token Generation and Validation
    
    private class AccountActionTokenPayload
    {
        public Guid UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
    }
    
    private string GenerateAccountActionToken(Guid userId, string action)
    {
        var payload = new AccountActionTokenPayload
        {
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            Action = action
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var payloadBytes = Encoding.UTF8.GetBytes(jsonPayload);

        using var hmac = new HMACSHA256(_secretKey);
        var signature = hmac.ComputeHash(payloadBytes);
        return $"{Convert.ToBase64String(payloadBytes)}.{Convert.ToBase64String(signature)}";
    }

    private bool ValidateAccountActionToken(string token, string expectedAction, out Guid userId)
    {
        userId = Guid.Empty;
        var parts = token.Split('.');
        if (parts.Length != 2)
            return false;

        try
        {
            var payloadBytes = Convert.FromBase64String(parts[0]);
            var signature = Convert.FromBase64String(parts[1]);

            using var hmac = new HMACSHA256(_secretKey);
            var expectedSignature = hmac.ComputeHash(payloadBytes);
            if (!expectedSignature.SequenceEqual(signature))
                return false;

            var payload = JsonSerializer.Deserialize<AccountActionTokenPayload>(Encoding.UTF8.GetString(payloadBytes));
            if (payload == null || payload.Action != expectedAction)
                return false;

            // Token expires in 30 minutes.
            if (DateTime.UtcNow > payload.Timestamp.AddMinutes(30))
                return false;

            userId = payload.UserId;
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion
    }