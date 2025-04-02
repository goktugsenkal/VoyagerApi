using Core.Entities;

namespace Core.Interfaces;

public interface ILikeRepository
{
    /// <summary>
    /// Checks if a like exists for the specified resource (Voyage or Comment) and user.
    /// </summary>
    /// <param name="voyageId">The ID of the voyage, or null if targeting a comment.</param>
    /// <param name="commentId">The ID of the comment, or null if targeting a voyage.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>True if the like exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(Guid? voyageId, Guid? commentId, Guid userId);

    /// <summary>
    /// Adds a like to the database.
    /// </summary>
    /// <param name="like">The like entity to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(Like like);

    /// <summary>
    /// Removes a like from the database.
    /// </summary>
    /// <param name="like">The like entity to remove.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveAsync(Like like);

    public Task<Like?> GetLikeAsync(Guid? voyageId, Guid? commentId, Guid userId);

    /// <summary>
    /// Retrieves all likes for a specified voyage or comment.
    /// </summary>
    /// <param name="voyageId">The ID of the voyage, or null if targeting a comment.</param>
    /// <param name="commentId">The ID of the comment, or null if targeting a voyage.</param>
    /// <returns>A list of likes.</returns>
    Task<List<Like>> GetLikesAsync(Guid? voyageId, Guid? commentId);

    /// <summary>
    /// Counts the number of likes for a specific voyage or comment.
    /// </summary>
    /// <param name="voyageId">The ID of the voyage, or null if targeting a comment.</param>
    /// <param name="commentId">The ID of the comment, or null if targeting a voyage.</param>
    /// <returns>The total count of likes.</returns>
    Task<int> CountLikesAsync(Guid? voyageId, Guid? commentId);
}