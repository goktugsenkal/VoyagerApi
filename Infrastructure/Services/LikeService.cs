using Core.Entities;
using Core.Enums;
using Core.Interfaces;

namespace Infrastructure.Services;

public class LikeService
    (ILikeRepository likeRepository, 
        ICommentRepository commentRepository, 
        IVoyageRepository voyageRepository) : ILikeService
// worst code in the world, doesn't run slow tho
// edit: might be brilliant code, I can't decide
{
    public async Task AddLikeToVoyageAsync(Guid voyageId, Guid userId)
    {
        // check if the user has already liked this voyage
        if (await likeRepository.ExistsAsync(voyageId: voyageId, commentId: null, userId: userId))
        {
            throw new InvalidOperationException("gok: user has already liked this voyage.");
        }

        // add a like to the likes table
        await likeRepository.AddAsync(new Like
        {
            VoyageId = voyageId,
            VoyagerUserId = userId,
            LikeType = LikeType.Voyage,
            CreatedAt = DateTime.UtcNow
        });
        
        // increment the like count of the voyage
        await voyageRepository.IncrementLikesAsync(voyageId);
    }

    public async Task RemoveLikeFromVoyageAsync(Guid voyageId, Guid userId)
    {
        // check if there is a like by thşis user to this voyage
        if (!await likeRepository.ExistsAsync(voyageId: voyageId, commentId: null, userId: userId))
        {
            throw new InvalidOperationException("gok: no like found from this user to this voyage.");
        }

        // get the like entity to be able to track it
        var like = await likeRepository.GetLikeAsync(voyageId: voyageId, commentId: null, userId: userId);
        
        await likeRepository.RemoveAsync(like!); // super sure that it is not null, because I check 2 lines above
        
        // decrement the like count of the voyage
        await voyageRepository.DecrementLikesAsync(voyageId);
    }

    public async Task AddLikeToCommentAsync(Guid commentId, Guid userId)
    {
        // check if the user has already liked this comment
        if (await likeRepository.ExistsAsync(voyageId: null, commentId: commentId, userId: userId))
        {
            throw new InvalidOperationException("gok: user has already liked this comment.");
        }

        // add a like to the likes table
        await likeRepository.AddAsync(new Like
        {
            CommentId = commentId,
            VoyagerUserId = userId,
            LikeType = LikeType.Comment,
            CreatedAt = DateTime.UtcNow
        });
        
        // increment the like count of the comment
        await commentRepository.IncrementLikesAsync(commentId);
    }

    public async Task RemoveLikeFromCommentAsync(Guid commentId, Guid userId)
    {
        // check if there is a like by this user to this voyage
        if (!await likeRepository.ExistsAsync(voyageId: null, commentId: commentId, userId: userId))
        {
            throw new InvalidOperationException("gok: no like found from this user to this voyage.");
        }

        // get the like entity to be able to track it
        var like = await likeRepository.GetLikeAsync(voyageId: null, commentId: commentId, userId: userId);
        
        await likeRepository.RemoveAsync(like!); // super sure that it is not null, because I check 2 lines above
        
        // decrement the like count of the comment
        await commentRepository.DecrementLikesAsync(commentId);
    }

    public async Task<int> CountLikesAsync(Guid? voyageId, Guid? commentId)
    {
        // make sure that at least one of voyageId or commentId is provided
        if (!voyageId.HasValue && !commentId.HasValue)
        {
            throw new ArgumentException("Either voyageId or commentId must be provided.");
        }
        
        // make sure that not both of them are provided
        if (voyageId.HasValue && commentId.HasValue)
        {
            throw new ArgumentException("Both voyageId and commentId cannot be provided together.");
        }

        // pass the shit down, 1 of them will almost definitely be null
        // which is checked thoroughly in the controller
        return await likeRepository.CountLikesAsync(voyageId, commentId);
    }


    public async Task<List<Like>> GetLikesAsync(Guid? voyageId, Guid? commentId)
    {
        // make sure that at least one of voyageId or commentId is provided
        if (!voyageId.HasValue && !commentId.HasValue)
        {
            throw new ArgumentException("Either voyageId or commentId must be provided.");
        }
        
        // make sure that not both of them are provided
        if (voyageId.HasValue && commentId.HasValue)
        {
            throw new ArgumentException("Both voyageId and commentId cannot be provided together.");
        }

        // pass the shit down as is again
        return await likeRepository.GetLikesAsync(voyageId, commentId);
    }

}