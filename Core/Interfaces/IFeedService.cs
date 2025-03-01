using Core.Dtos;
using Core.Models;

namespace Core.Interfaces;

public interface IFeedService
{
    Task<PagedList<VoyageDto>> GetFeedAsync(int pageNumber, int pageSize);
}