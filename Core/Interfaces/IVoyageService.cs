using Core.Dtos;
using Core.Models;

namespace Core.Interfaces;

public interface IVoyageService
{
    Task<List<VoyageDto>> GetAllVoyagesAsync();
    Task<VoyageDto?> GetVoyageByIdAsync(Guid voyageId);
    Task AddVoyageAsync(CreateVoyageModel createVoyageModel, Guid voyagerUserId);
    Task UpdateVoyageAsync(Guid voyageId, UpdateVoyageModel updateVoyageModel);
    Task DeleteVoyageAsync(Guid voyageId);
}