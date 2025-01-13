using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LikeRepository(DataContext context) : ILikeRepository
{
    public async Task<bool> ExistsAsync(Guid? voyageId, Guid? commentId, Guid userId)
    {
        // returns true if
        // the provided user has liked the specified voyage (when voyageId is not null).
        // OR
        // the provided user has liked the specified comment (when commentId is not null).
        // userId and commentId SHOULD NEVER BE PROVIDED TOGETHER!!!
        return await context.Likes.AnyAsync(l =>
            l.VoyagerUserId == userId && 
            (voyageId.HasValue && l.VoyageId == voyageId 
             || 
             commentId.HasValue && l.CommentId == commentId)
        );
    }
    
    public async Task AddAsync(Like like)
    {
        await context.Likes.AddAsync(like);
        await context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Like like)
    {
        context.Likes.Remove(like);
        await context.SaveChangesAsync();
    }

    public async Task<Like?> GetLikeAsync(Guid? voyageId, Guid? commentId, Guid userId)
    {
        return await context.Likes.FirstOrDefaultAsync(l =>
            l.VoyagerUserId == userId &&
            ((voyageId.HasValue && l.VoyageId == voyageId) || 
             (commentId.HasValue && l.CommentId == commentId))
        );
    }



    public async Task<List<Like>> GetLikesAsync(Guid? voyageId, Guid? commentId)
    {
        return await context.Likes
            .Where(l =>
                (voyageId.HasValue && l.VoyageId == voyageId) ||
                (commentId.HasValue && l.CommentId == commentId)
            )
            .ToListAsync();
    }

    public async Task<int> CountLikesAsync(Guid? voyageId, Guid? commentId)
    {
        return await context.Likes
            .CountAsync(l =>
                (voyageId.HasValue && l.VoyageId == voyageId) ||
                (commentId.HasValue && l.CommentId == commentId)
            );
    }

}