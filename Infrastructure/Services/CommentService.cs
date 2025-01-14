using Core.Entities;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services;

public class CommentService(ICommentRepository commentRepository) : ICommentService
{
    public async Task AddCommentAsync(Guid voyageId, Guid voyagerUserId, CreateCommentModel commentModel)
    {
        var comment = new Comment
        {
            VoyageId = voyageId,
            VoyagerUserId = voyagerUserId,
            Content = commentModel.Content,
            CreatedAt = DateTime.UtcNow
        };
        
        try
        {
            await commentRepository.AddCommentAsync(comment);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        } 
    }

    public async Task DeleteCommentAsync(Guid voyageId, Guid voyagerUserId, Guid commentId)
    {
        throw new NotImplementedException();
    }
}