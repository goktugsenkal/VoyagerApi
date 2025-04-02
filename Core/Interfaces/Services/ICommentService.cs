using Core.Entities;
using Core.Models;

namespace Core.Interfaces;

/// <summary>
/// Interface for handling comment-related operations.
/// </summary>
public interface ICommentService
{
    /// <summary>
    /// Retrieves comments for a specified voyage.
    /// </summary>
    /// <param name="voyageId">The unique identifier of the voyage.</param>
    /// <returns>A list of comments for the specified voyage.</returns>
    Task<List<Comment>> GetCommentsByVoyageIdAsync(Guid voyageId);
    
    /// <summary>
    /// Adds a comment to a specified voyage, associates it with the specified userId.
    /// </summary>
    /// <param name="voyageId">The unique identifier of the voyage.</param>
    /// <param name="voyagerUserId">The unique identifier of the user creating the comment.</param>
    /// <param name="commentModel">The model containing the comment details.</param>
    Task AddCommentAsync(Guid voyageId, Guid voyagerUserId, CreateCommentModel commentModel);

    /// <summary>
    /// Updates a comment on a specified voyage, associated with the specified userId.
    /// </summary>
    /// <param name="voyageId">The unique identifier of the voyage.</param>
    /// <param name="voyagerUserId">The unique identifier of the user updating the comment.</param>
    /// <param name="commentId">The unique identifier of the comment to be updated.</param>
    /// <param name="commentModel">The model containing the updated comment details.</param>
    Task UpdateCommentAsync(Guid voyageId, Guid voyagerUserId, Guid commentId, CreateCommentModel commentModel);

    /// <summary>
    /// Deletes a comment from a specified voyage, associated with the specified userId.
    /// </summary>
    /// <param name="voyagerUserId">The unique identifier of the user deleting the comment.</param>
    /// <param name="commentId">The unique identifier of the comment to be deleted.</param>
    Task DeleteCommentAsync(Guid voyagerUserId, Guid commentId);
    
}