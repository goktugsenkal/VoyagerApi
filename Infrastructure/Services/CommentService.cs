using Core.Entities;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services;

public class CommentService(ICommentRepository commentRepository, IVoyageRepository voyageRepository) : ICommentService
{
    public async Task<List<Comment>> GetCommentsByVoyageIdAsync(Guid voyageId)
    {
        return await commentRepository.GetCommentsAsync(voyageId);
    }

    public async Task AddCommentAsync(Guid voyageId, Guid voyagerUserId, CreateCommentModel commentModel)
    {
        // Create comment
        var comment = new Comment
        {
            VoyageId = voyageId,
            VoyagerUserId = voyagerUserId,
            Content = commentModel.Content,
            CreatedAt = DateTime.UtcNow
        };
        
        try
        {
            // Save comment
            await commentRepository.AddAsync(comment);

            // Increment comments count
            await voyageRepository.IncrementCommentsAsync(voyageId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        } 
    }

    public async Task DeleteCommentAsync(Guid voyageId, Guid voyagerUserId, Guid commentId)
    {
        // Get comment
        var comment = await commentRepository.GetCommentByIdAsync(commentId);
        
        // Check if comment exists
        if (comment is null)
        {
            throw new Exception("Comment not found.");
        }
        
        // Check if comment belongs to user
        if (comment.VoyageId != voyageId || comment.VoyagerUserId != voyagerUserId)
        {
            throw new Exception("You are not authorized to delete this comment.");
        }
        
        // Delete comment
        await commentRepository.DeleteCommentAsync(comment);
        
        // Decrement comments count
        await voyageRepository.DecrementCommentsAsync(voyageId);
    }
}