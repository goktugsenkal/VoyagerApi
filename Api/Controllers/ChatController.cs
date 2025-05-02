using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Models.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("chat")]
[ApiController]
public class ChatController(IChatService chatService) : BaseApiController
{
    [Authorize]
    [HttpPost("signup")]
    public async Task<IActionResult> SignUpForChat()
    {
        var userId = GetUserIdFromTokenClaims();
        if (userId is null)
            return Unauthorized("User ID not found in token claims.");

        try
        {
            await chatService.SignUpForChatAsync(userId.Value); // value is checked for null
            return Ok(new { message = "Chat registration successful." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    [HttpPost("rooms")]
    public async Task<IActionResult> CreateChatRoom([FromBody] CreateChatRoomModel model)
    {
        try
        {
            // validate media types
            ValidateMediaTypes(model.ImageType);
            ValidateMediaTypes(model.BannerType);
            
            var result = await chatService.CreateChatRoomAsync(model);
            return Ok(result);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static readonly HashSet<string> AllowedMediaTypes = new()
    {
        "mp4", "jpg", "jpeg", "png", "gif"
    };

    private void ValidateMediaTypes(string mediaType)
    {
        if (!AllowedMediaTypes.Contains(mediaType.ToLower()))
            throw new InvalidOperationException($"Unsupported media type: {mediaType}");
    }
}