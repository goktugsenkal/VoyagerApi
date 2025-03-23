using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services;

public class FeedService(IVoyageRepository voyageRepository,
        IS3Service s3Service,
        ICommentRepository commentRepository, 
        ILikeRepository likeRepository) : IFeedService
{
    public async Task<PagedList<VoyageDto>> GetFeedAsync(int pageNumber, int pageSize)
    {
        // get all voyages with paged list
        var voyages = voyageRepository.GetAllAsPagedList(pageNumber, pageSize);

        // map voyages to VoyageDtos
        var voyagesDtos = voyages.Items.Select(voyage => voyage.ToDto()).ToList();
        
        // convert image keys to s3 presigned download urls
        foreach (var voyageDto in voyagesDtos)
        {
            if (voyageDto.ImageUrls != null && voyageDto.ImageUrls.Any())
            {
                voyageDto.ImageUrls = voyageDto.ImageUrls
                    .Select(key => s3Service.GeneratePreSignedDownloadUrl(key, TimeSpan.FromMinutes(15)))
                    .ToList();
            }
    
            if (voyageDto.Stops != null && voyageDto.Stops.Any())
            {
                foreach (var stopDto in voyageDto.Stops)
                {
                    if (stopDto.ImageUrls != null && stopDto.ImageUrls.Any())
                    {
                        stopDto.ImageUrls = stopDto.ImageUrls
                            .Select(key => s3Service.GeneratePreSignedDownloadUrl(key, TimeSpan.FromMinutes(15)))
                            .ToList();
                    }
                }
            }
        }
        
        // create a PagedList and return it
        return new PagedList<VoyageDto>(voyagesDtos, voyages.TotalCount, voyages.CurrentPage, voyages.PageSize);
    }
}