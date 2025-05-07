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
    
    [HttpPost("test")]
    public async Task<IActionResult> SendTestNotification([FromBody] TestNotificationDto dto)
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
}

public class TestNotificationDto
{
    public Guid UserId { get; set; }
    public string Title   { get; set; } = string.Empty;
    public string Body    { get; set; } = string.Empty;
}