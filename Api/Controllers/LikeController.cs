using System.Security.Claims;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("like")]
public class LikeController(ILikeService likeService) : ControllerBase
{
    /// <summary>
    /// like a voyage
    /// </summary>
    /// <param name="voyageId"></param>
    /// <param name="commentId"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddLikeAsync([FromQuery] Guid? voyageId, [FromQuery] Guid? commentId)
    {
        // check Authorization header, then try to find the User's ID in the claims 
        var voyagerUserIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        
        // return 401 if User's ID can't be found
        if (voyagerUserIdClaim == null)
        {
            return Unauthorized("User ID not found in the token.");
        }
        
        // parse User's ID into a Guid
        var voyagerUserId = Guid.Parse(voyagerUserIdClaim.Value);
        
        // if neither query parameter is provided
        if (voyageId == null && commentId == null)
        {
            return BadRequest("Either voyageId or commentId must be provided.");
        }

        // if both query parameters are provided
        if (voyageId != null && commentId != null)
        {
            return BadRequest("Provide either a voyageId or a commentId, not both.");
        }

        try
        {
            // if voyageId is provided, call likeService with voyageId and User's ID
            if (voyageId != null)
            {
                await likeService.AddLikeToVoyageAsync(voyageId.Value, voyagerUserId);
            }
            // if commentId is provided, call likeService with commentId and User's ID
            else if (commentId != null)
            {
                await likeService.AddLikeToCommentAsync(commentId.Value, voyagerUserId);
            }
            // a like will be created accordingly and added to the database

            return Ok("Like added successfully.");
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
        }
    }

    /// <summary>
    /// unlike a voyage
    /// </summary>
    /// <param name="voyageId"></param>
    /// <param name="commentId"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<IActionResult> RemoveLikeAsync(Guid? voyageId, Guid? commentId)
    {
        // check Authorization header, then try to find the User's ID in the claims 
        var voyagerUserIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        
        // return 401 if User's ID can't be found
        if (voyagerUserIdClaim == null)
        {
            return Unauthorized("User ID not found in the token.");
        }
        
        // parse User's ID into a Guid
        var voyagerUserId = Guid.Parse(voyagerUserIdClaim.Value);
        
        // if neither query parameter is provided
        if (voyageId == null && commentId == null)
        {
            return BadRequest("Either voyageId or commentId must be provided.");
        }

        // if both query parameters are provided
        if (voyageId != null && commentId != null)
        {
            return BadRequest("Provide either a voyageId or a commentId, not both.");
        }

        try
        {
            // if voyageId is provided, call likeService with voyageId and User's ID
            if (voyageId != null)
            {
                await likeService.RemoveLikeFromVoyageAsync(voyageId.Value, voyagerUserId);
            }
            // if commentId is provided, call likeService with commentId and User's ID
            else if (commentId != null)
            {
                await likeService.RemoveLikeFromCommentAsync(commentId.Value, voyagerUserId);
            }
            // a like will be removed accordingly and updated in the database

            return Ok("Like removed successfully.");
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
        }
    }
}