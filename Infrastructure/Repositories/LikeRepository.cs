using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LikeRepository(DataContext context) : ILikeRepository
{
    /// <summary>
    /// returns true if
    /// the provided user has liked the specified voyage (when voyageId is not null).
    /// OR
    /// the provided user has liked the specified comment (when commentId is not null).
    /// userId and commentId SHOULD NEVER BE PROVIDED TOGETHER!!!
    /// </summary>
    public async Task<bool> ExistsAsync(Guid? voyageId, Guid? commentId, Guid userId)
    {
        return await context.Likes.AnyAsync(l =>
            l.VoyagerUserId == userId && 
            (voyageId.HasValue && l.VoyageId == voyageId 
             || 
             commentId.HasValue && l.CommentId == commentId)
        );
    }
    
    /// <summary>
    /// adds a new like to the likes table
    /// </summary>
    public async Task AddAsync(Like like)
    {
        like.CreatedAt = DateTime.UtcNow;
        await context.Likes.AddAsync(like);
        await context.SaveChangesAsync();
    }   

    /// <summary>
    /// removes a like from the likes table
    /// </summary>
    public async Task RemoveAsync(Like like)
    {
        context.Likes.Remove(like);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// returns the like entity if it exists, null otherwise
    /// </summary>
    public async Task<Like?> GetLikeAsync(Guid? voyageId, Guid? commentId, Guid userId)
    {
        return await context.Likes.FirstOrDefaultAsync(l =>
            l.VoyagerUserId == userId &&
            ((voyageId.HasValue && l.VoyageId == voyageId) || 
             (commentId.HasValue && l.CommentId == commentId))
        );
    }



    /// <summary>
    /// returns a list of likes for the specified voyage or comment
    /// </summary>
    public async Task<List<Like>> GetLikesAsync(Guid? voyageId, Guid? commentId)
    {
        return await context.Likes
            .Where(l =>
                (voyageId.HasValue && l.VoyageId == voyageId) ||
                (commentId.HasValue && l.CommentId == commentId)
            )
            .ToListAsync();
    }

    /// <summary>
    /// returns the count of likes for the specified voyage or comment
    /// </summary>
    public async Task<int> CountLikesAsync(Guid? voyageId, Guid? commentId)
    {
        return await context.Likes
            .CountAsync(l =>
                (voyageId.HasValue && l.VoyageId == voyageId) ||
                (commentId.HasValue && l.CommentId == commentId)
            );
    }

}