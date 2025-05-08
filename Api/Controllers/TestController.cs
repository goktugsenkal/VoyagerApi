using Core.Constants;
using Core.Interfaces;
using Core.Interfaces.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("version")]
public class TestController(DataContext context, IFcmService fcmService, ILogger<TestController> logger) : BaseApiController
{
    [HttpGet]
    public ActionResult<string> GetVersion()
    {
        return Content(VersionInfo.ApiVersion, "text/plain");
    }
    
    [HttpPost("fg-notification-test")]
    public async Task<IActionResult> SendFgTestNotification([FromBody] TestNotificationDto dto)
    {
        // fetch the user (and their FCM token) from the database
        var userTokens = await context.UserSessions
            .Where(rt => rt.UserId == dto.UserId && rt.FcmToken != null && rt.RevokedAt == null)
            .Select(rt => rt.FcmToken)
            .ToListAsync();

        
        var messageIds = new List<string>();
        // send the push notification to that token
        foreach (var token in userTokens)
        {
            logger.LogDebug("Sending notification to token: {Token}", token);
            var messageId = await fcmService.SendNotificationAsync(
                token!,         // the device token
                dto.Title,              // notification title
                dto.Body,               // notification body
                new Dictionary<string,string> { ["source"] = "test-endpoint" } 
            );
            
            messageIds.Add(messageId);
        }

        // return the FCM message ID for debugging
        return Ok(new { MessageId = messageIds });
    }
    
    [HttpPost("bg-notification-test")]
    public async Task<IActionResult> SendBgTestNotification([FromBody] TestNotificationDto dto)
    {
        // fetch all active FCM tokens for this user
        var userTokens = await context.UserSessions
            .Where(us => us.UserId   == dto.UserId
                         && us.FcmToken != null
                         && us.RevokedAt == null)
            .Select(us => us.FcmToken!)
            .ToListAsync();

        var messageIds = new List<string>();

        // build a single data‐payload dictionary from the DTO
        var data = new Dictionary<string, string>
        {
            ["action"]    = dto.Action.ToString(),            // custom action code
            ["messageId"] = dto.MessageId.ToString(),         // link back to message
            ["userId"]    = dto.UserId.ToString(),            // sender id
            ["roomId"]    = dto.RoomId.ToString(),            // chat room id
            ["timestamp"] = dto.Timestamp.ToString("o")       // ISO 8601 timestamp
        };

        // include voyageId only if present
        if (!string.IsNullOrWhiteSpace(dto.VoyageId))
            data["voyageId"] = dto.VoyageId;                  // optional voyage link

        // send one background push per device token
        foreach (var token in userTokens)
        {
            // sendBackgroundNotificationAsync wraps title/body → FCM Notification and data → FCM Data
            var fcmMessageId = await fcmService
                .SendBackgroundNotificationAsync(token, data);

            messageIds.Add(fcmMessageId);
        }

        // return list of FCM message IDs for debugging
        return Ok(new { MessageIds = messageIds });
    }
}

public class TestNotificationDto
{
    public string Title   { get; set; } = string.Empty;
    public string Body    { get; set; } = string.Empty;
    public short Action { get; set; }
    public Guid? MessageId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? RoomId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    
    public string? VoyageId { get; set; }
}