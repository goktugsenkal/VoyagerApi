using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("like")]
public class LikeController : ControllerBase
{
    /// <summary>
    /// like a voyage
    /// </summary>
    /// <param name="voyageId"></param>
    /// <returns></returns>
    [HttpPost("voyage/{voyageId:guid}")]
    public async Task<IActionResult> AddLikeToVoyage(Guid voyageId)
    {
        // Logic to add a like to the voyage.
        return Ok();
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