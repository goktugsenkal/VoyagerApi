using Core.Dtos;
using Core.Models;

namespace Core.Interfaces;

public interface IVoyageService
{
    Task<PagedList<VoyageDto>> GetAllVoyagesAsync(int pageNumber, int pageSize);

    PagedList<VoyageDto> GetVoyagesFiltered(double? latitudeMin, double? latitudeMax, double? longitudeMin,
        double? longitudeMax, int pageNumber, int pageSize);

    Task<PagedList<VoyageDto>> GetVoyagesByVoyagerUserIdAsync(Guid voyagerUserId, int pageNumber, int pageSize);

    Task<VoyageDto?> GetVoyageByIdAsync(Guid voyageId);
    Task AddVoyageAsync(CreateVoyageModel createVoyageModel, Guid voyagerUserId);
    Task UpdateVoyageAsync(Guid voyageId, UpdateVoyageModel updateVoyageModel);
    Task DeleteVoyageAsync(Guid voyageId);
}