using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("comments")]
public class CommentController(ICommentService commentService) : BaseApiController
{
    /// <summary>
    /// Get comments for a given voyage.
    /// </summary>
    /// <param name="voyageId">The ID of the voyage to get comments for.</param>
    /// <returns>A list of comments for the given voyage.</returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetComments(Guid voyageId)
    {
        var voyagerUserIdClaim = GetUserIdFromTokenClaims();
        
        // return 401 if User's ID can't be found
        if (voyagerUserIdClaim == null)
        {
            return Unauthorized("User ID not found in the token.");
        }
        
        var comments = await commentService.GetCommentsByVoyageIdAsync(voyageId);
        // null case is checked, so it's okay to use voyagerUserId.Value

        return Ok(comments);
    }
    
    /// <summary>
    /// Adds a comment to a specified voyage. This endpoint requires authorization and retrieves the user ID from the token claims.
    /// </summary>
    /// <param name="voyageId">The ID of the voyage to which the comment is being added.</param>
    /// <param name="commentModel">The model containing the details of the comment to be added.</param>
    /// <returns>Returns a success message if the comment is added successfully, otherwise returns an error message.</returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddComment(Guid voyageId, CreateCommentModel commentModel)
    {
        // check Authorization header, then try to find the User's ID in the claims 
        var voyagerUserId = GetUserIdFromTokenClaims();
        
        // return 401 if User's ID can't be found
        if (voyagerUserId == null)
        {
            return Unauthorized("User ID not found in the token.");
        }
        
        try
        {
            // call VoyageService to do the saving of the Voyage and Stops
            // give down voyageUserId that we got from the token's claims
            await commentService.AddCommentAsync(voyageId, voyagerUserId.Value, commentModel);
            // null case is checked, so it's okay to use voyagerUserId.Value

            return Ok("Comment created successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Updates an existing comment for a specified voyage. This endpoint requires authorization
    /// and retrieves the user ID from the token claims to ensure the user is authorized to perform the update.
    /// </summary>
    /// <param name="voyageId">The ID of the voyage associated with the comment to be updated.</param>
    /// <param name="commentId">The ID of the comment to be updated.</param>
    /// <param name="commentModel">The model containing the updated details of the comment.</param>
    /// <returns>Returns a success message if the comment is updated successfully, otherwise returns an error message.</returns>
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateComment(Guid voyageId, Guid commentId, CreateCommentModel commentModel)
    {
        // check Authorization header, then try to find the User's ID in the claims 
        var voyagerUserId = GetUserIdFromTokenClaims();
        
        // return 401 if User's ID can't be found
        if (voyagerUserId == null)
        {
            return Unauthorized("User ID not found in the token.");
        }
        
        try
        {
            // call VoyageService to do the saving of the Voyage and Stops
            // give down voyageUserId that we got from the token's claims
            await commentService.UpdateCommentAsync(voyageId, commentId, voyagerUserId.Value, commentModel);
            // null case is checked, so it's okay to use voyagerUserId.Value

            return Ok("Comment updated successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Deletes a comment associated with a specified voyage. 
    /// This endpoint requires authorization and retrieves the user ID from the token claims 
    /// to ensure the user is authorized to perform the deletion.
    /// </summary>
    /// <param name="commentId">The ID of the comment to be deleted.</param>
    /// <returns>Returns a success message if the comment is deleted successfully, otherwise returns an error message.</returns>
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        // check Authorization header, then try to find the User's ID in the claims 
        var voyagerUserId = GetUserIdFromTokenClaims();
        
        // return 401 if User's ID can't be found
        if (voyagerUserId == null)
        {
            return Unauthorized("User ID not found in the token.");
        }
        
        try
        {
            // call VoyageService to do the saving of the Voyage and Stops
            // give down voyageUserId that we got from the token's claims
            await commentService.DeleteCommentAsync(commentId, voyagerUserId.Value);
            // null case is checked, so it's okay to use voyagerUserId.Value
            
            return Ok("Comment deleted successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}