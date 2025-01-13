using Core.Entities;

namespace Core.Interfaces;

/// <summary>
/// Provides services for managing likes on voyages and comments, including adding, removing, and querying like information.
/// This service handles the business logic for like operations while enforcing validation rules to prevent duplicate likes
/// and ensuring data consistency.
/// </summary>
public interface ILikeService
{
    /// <summary>
    /// Adds a like to a voyage by a user.
    /// </summary>
    /// <param name="voyageId">The ID of the voyage being liked.</param>
    /// <param name="userId">The ID of the user who is liking the voyage.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddLikeToVoyageAsync(Guid voyageId, Guid userId);

    /// <summary>
    /// Removes a like from a voyage by a user.
    /// </summary>
    /// <param name="voyageId">The ID of the voyage being unliked.</param>
    /// <param name="userId">The ID of the user who is unliking the voyage.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveLikeFromVoyageAsync(Guid voyageId, Guid userId);

    /// <summary>
    /// Adds a like to a comment by a user.
    /// </summary>
    /// <param name="commentId">The ID of the comment being liked.</param>
    /// <param name="userId">The ID of the user who is liking the comment.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddLikeToCommentAsync(Guid commentId, Guid userId);

    /// <summary>
    /// Removes a like from a comment by a user.
    /// </summary>
    /// <param name="commentId">The ID of the comment being unliked.</param>
    /// <param name="userId">The ID of the user who is unliking the comment.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveLikeFromCommentAsync(Guid commentId, Guid userId);

    /// <summary>
    /// Retrieves the count of likes for a specified voyage or comment.
    /// </summary>
    /// <param name="voyageId">The ID of the voyage (optional).</param>
    /// <param name="commentId">The ID of the comment (optional).</param>
    /// <returns>The count of likes for the specified resource.</returns>
    Task<int> CountLikesAsync(Guid? voyageId, Guid? commentId);

    /// <summary>
    /// Retrieves a list of likes for a specified voyage or comment.
    /// </summary>
    /// <param name="voyageId">The ID of the voyage (optional).</param>
    /// <param name="commentId">The ID of the comment (optional).</param>
    /// <returns>A list of likes for the specified resource.</returns>
    Task<List<Like>> GetLikesAsync(Guid? voyageId, Guid? commentId);
}
