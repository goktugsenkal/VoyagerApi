using Core.Entities;

namespace Core.Interfaces;

public interface IVoyageRepository
{
    public Task<ICollection<Voyage>> GetAllVoyages();
    public Task<Voyage?> GetVoyageById(Guid voyageId);
    Task AddAsync(Voyage voyage);
    public Task UpdateVoyage(Voyage voyage);
    public Task DeleteVoyage(Voyage voyage);
    
    Task<bool> IncrementLikesAsync(Guid voyageId);
    Task<bool> DecrementLikesAsync(Guid voyageId);
    
    Task<bool> IncrementCommentsAsync(Guid voyageId);
    Task<bool> DecrementCommentsAsync(Guid voyageId);
}