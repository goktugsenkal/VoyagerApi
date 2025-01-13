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
    [HttpPost("add")]
    [Authorize]
    public async Task<IActionResult> AddLikeAsync(Guid? voyageId, Guid? commentId)
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
        
        if (voyageId == null && commentId == null)
        {
            return BadRequest("Either voyageId or commentId must be provided.");
        }

        if (voyageId != null && commentId != null)
        {
            return BadRequest("Provide either a voyageId or a commentId, not both.");
        }

        try
        {
            if (voyageId != null)
            {
                await likeService.AddLikeToVoyageAsync(voyageId.Value, voyagerUserId);
            }
            else if (commentId != null)
            {
                await likeService.AddLikeToCommentAsync(commentId.Value, voyagerUserId);
            }

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
    /// <returns></returns>
    [HttpDelete("voyage/{voyageId:guid}")]
    public async Task<IActionResult> RemoveLikeFromVoyage(Guid voyageId)
    {
        // Logic to remove a like from the voyage.
        return NoContent();
    }

    /// <summary>
    /// like a comment
    /// </summary>
    /// <param name="commentId"></param>
    /// <returns></returns>
    [HttpPost("comments/{commentId:guid}")]
    public async Task<IActionResult> AddLikeToComment(Guid commentId)
    {
        // Logic to add a like to the comment.
        return Ok();
    }

    /// <summary>
    /// unlike a comment
    /// </summary>
    /// <param name="commentId"></param>
    /// <returns></returns>
    [HttpDelete("comments/{commentId:guid}")]
    public async Task<IActionResult> RemoveLikeFromComment(Guid commentId)
    {
        // Logic to remove a like from the comment.
        return NoContent();
    }
}