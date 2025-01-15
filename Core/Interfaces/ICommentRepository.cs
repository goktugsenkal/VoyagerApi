using Core.Entities;

namespace Core.Interfaces;

/// <summary>
///     Interface for comment repository.
/// </summary>
public interface ICommentRepository
{
    /// <summary>
    ///     Retrieves a list of comments for a given voyage.
    /// </summary>
    /// <param name="voyageId">The ID of the voyage to retrieve comments for.</param>
    /// <returns>A list of comments for the given voyage.</returns>
    Task<List<Comment>> GetCommentsAsync(Guid voyageId);

    /// <summary>
    ///     Retrieves a comment by its ID.
    /// </summary>
    /// <param name="commentId">The ID of the comment to retrieve.</param>
    /// <returns>The comment if found, otherwise null.</returns>
    Task<Comment?> GetCommentByIdAsync(Guid commentId);

    /// <summary>
    ///     Updates a comment.
    /// </summary>
    /// <param name="comment">The comment to update.</param>
    Task UpdateAsync(Comment comment);

    /// <summary>
    ///     Adds a new comment.
    /// </summary>
    /// <param name="comment">The comment to add.</param>
    Task AddAsync(Comment comment);

    /// <summary>
    ///     Deletes a comment asynchronously.
    /// </summary>
    /// <param name="comment">The comment to delete.</param>
    Task DeleteCommentAsync(Comment comment);

    /// <summary>
    ///     Increments the likes count of a comment asynchronously.
    /// </summary>
    /// <param name="commentId">The ID of the comment to increment the likes count for.</param>
    /// <returns>True if the increment was successful, otherwise false.</returns>
    Task<bool> IncrementLikesAsync(Guid commentId);

    /// <summary>
    ///     Decrements the likes count of a comment asynchronously.
    /// </summary>
    /// <param name="commentId">The ID of the comment to decrement the likes count for.</param>
    /// <returns>True if the decrement was successful, otherwise false.</returns>
    Task<bool> DecrementLikesAsync(Guid commentId);
}