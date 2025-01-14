using Core.Entities;

namespace Core.Interfaces;

public interface ICommentRepository
{
    Task<List<Comment>> GetCommentsByVoyageIdAsync(Guid voyageId);
    Task<Comment?> GetCommentByIdAsync(Guid commentId);
    Task AddCommentAsync(Comment comment);
    Task UpdateCommentAsync(Comment comment);
    Task DeleteCommentAsync(Comment comment);
    
    Task<bool> IncrementLikesAsync(Guid commentId);
    Task<bool> DecrementLikesAsync(Guid commentId);
}