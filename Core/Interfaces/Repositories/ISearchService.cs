using Core.Dtos;

namespace Core.Interfaces.Repositories;

public interface ISearchService
{
    Task<List<SearchResultDto>> GlobalSearchAsync(string query);
}
