using Core.Entities;

namespace Core.Interfaces;

public interface IVoyageRepository
{
    public Task<ICollection<Voyage>> GetAllVoyages();
    public Task<Voyage> GetVoyageById(Guid voyageId);
    public Task CreateVoyage(Voyage voyage);
    public Task UpdateVoyage(Voyage voyage);
    public Task DeleteVoyage(Voyage voyage);
}