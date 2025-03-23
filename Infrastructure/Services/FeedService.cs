using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services;

public class FeedService(IVoyageRepository voyageRepository, 
        ICommentRepository commentRepository, 
        ILikeRepository likeRepository) : IFeedService
{
    public async Task<PagedList<VoyageDto>> GetFeedAsync(int pageNumber, int pageSize)
    {
        // get all voyages with paged list
        var voyages = voyageRepository.GetAllAsPagedList(pageNumber, pageSize);

        // map voyages to VoyageDtos
        var voyagesDtos = voyages.Items.Select(voyage => voyage.ToDto()).ToList();
        
        // create a PagedList and return it
        return new PagedList<VoyageDto>(voyagesDtos, voyages.TotalCount, voyages.CurrentPage, voyages.PageSize);
    }
}