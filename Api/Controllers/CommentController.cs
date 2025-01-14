using System.Security.Claims;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("comment")]
public class CommentController(ICommentService commentService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetComments(Guid voyageId)
    {
        return Ok();
    }
    
    [HttpPost]
    [Authorize]
    public IActionResult AddComment(Guid voyageId, CreateCommentModel commentModel)
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
        
        try
        {
            // call VoyageService to do the saving of the Voyage and Stops
            // give down voyageUserId that we got from the token's claims
            commentService.AddCommentAsync(voyageId, voyagerUserId, commentModel);
            
            return Ok("Comment created successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}