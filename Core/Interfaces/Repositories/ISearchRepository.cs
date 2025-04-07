using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface ISearchRepository
{
    Task<List<Voyage>> SearchVoyagesAsync(string query);
    Task<List<Voyage>> SearchVoyagesByStopMatchAsync(string query);
    Task<List<VoyagerUser>> SearchUsersAsync(string query);
}
