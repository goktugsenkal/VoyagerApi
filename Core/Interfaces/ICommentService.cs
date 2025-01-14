using Core.Models;

namespace Core.Interfaces;

public interface ICommentService
{
    Task AddCommentAsync(Guid voyageId, Guid voyagerUserId, CreateCommentModel commentModel);
    Task DeleteCommentAsync(Guid voyageId, Guid voyagerUserId, Guid commentId);
}