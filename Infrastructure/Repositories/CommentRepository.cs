using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CommentRepository(DataContext context) : ICommentRepository
{
    public async Task<List<Comment>> GetCommentsAsync(Guid voyageId)
    {
        return await context.Comments
            .Where(c => c.VoyageId == voyageId)
            .ToListAsync();
    }

    public async Task<Comment?> GetCommentByIdAsync(Guid commentId)
    {
        return await context.Comments
            .FirstOrDefaultAsync(c => c.Id == commentId);
    }
    
    public async Task UpdateAsync(Comment comment)
    {
        context.Comments.Update(comment);
        await context.SaveChangesAsync();
    }

    public async Task AddAsync(Comment comment)
    {
        comment.CreatedAt = DateTime.UtcNow;
        await context.Comments.AddAsync(comment);
        await context.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(Comment comment)
    {
        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
    }

    public async Task<bool> IncrementLikesAsync(Guid commentId)
    {
        var comment = await GetCommentByIdAsync(commentId);
        
        if (comment is null)
        {
            return false;
        }
        
        comment.LikeCount++;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DecrementLikesAsync(Guid commentId)
    {
        var comment = await GetCommentByIdAsync(commentId);
        
        if (comment is null)
        {
            return false;
        }
        
        comment.LikeCount--;
        await context.SaveChangesAsync();
        return true;
    }
}