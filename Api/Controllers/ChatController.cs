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
    public async Task<IActionResult> SignUpForChat(SignUpForChatModel model)
    {
        var userId = GetUserIdFromTokenClaims();
        if (userId is null)
            return Unauthorized("User ID not found in token claims.");

        try
        {
            await chatService.SignUpForChatAsync(userId.Value, model); // userId value is checked for null
            return Ok(new { message = "Chat registration successful." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
    
    [Authorize]
    [HttpPost("rooms")]
    public async Task<IActionResult> CreateChatRoom([FromBody] CreateChatRoomModel model)
    {
        var userId = GetUserIdFromTokenClaims();
        if (userId == null)
            return Unauthorized("User ID not found in token claims.");
        
        try
        {
            // validate media types
            ValidateMediaTypes(model.ImageType);
            ValidateMediaTypes(model.BannerType);
            
            // add the creator as an admin
            model.ParticipantModels.Add(new CreateChatRoomParticipantModel
            {
                IsAdmin = true,
                UserId = userId.Value
            });
            
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
    
    [Authorize]
    [HttpGet("rooms")]
    public async Task<IActionResult> GetChatRooms()
    {
        var userId = GetUserIdFromTokenClaims();
        if (userId == null)
            return Unauthorized("User ID not found in token claims.");
        
        var result = await chatService.GetChatRoomsForUserAsync(userId.Value, 1, int.MaxValue);
        return Ok(result);
    }

    private static readonly HashSet<string> AllowedMediaTypes = ["mp4", "jpg", "jpeg", "png", "gif"];

    private void ValidateMediaTypes(string mediaType)
    {
        if (!AllowedMediaTypes.Contains(mediaType.ToLower()))
            throw new InvalidOperationException($"Unsupported media type: {mediaType}");
    }

    [Authorize]
    [HttpPost("rooms/{roomId}/participants")]
    public async Task<IActionResult> AddChatRoomParticipants(string roomId, List<CreateChatRoomParticipantModel> models)
    {
        var userId = GetUserIdFromTokenClaims();
        if (userId == null)
            return Unauthorized("User ID not found in token claims.");

        if (!Guid.TryParse(roomId, out var roomGuid))
            return BadRequest("Invalid room ID format.");

        if (models.Count == 0)
            return BadRequest("No participants provided.");

        try
        {
            foreach (var model in models)
            {
                await chatService.AddChatRoomParticipantAsync(roomGuid, model);
            }
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("/peers/online")]
    public async Task<IActionResult> GetOnlinePeers()
    {
        var userId = GetUserIdFromTokenClaims();
        if (userId == null)
            return Unauthorized("User ID not found in token claims.");
        
        var result = await chatService.GetOnlinePeersAsyncForUserAsync(userId.Value);
        return Ok(result);
    }
}