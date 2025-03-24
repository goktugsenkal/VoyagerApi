using Core.Dtos;
using Core.Entities;
using Core.Models;

namespace Core.Interfaces;

public interface IVoyageService
{
    Task<PagedList<VoyageDto>> GetAllVoyagesAsync(int pageNumber, int pageSize);

    PagedList<VoyageDto> GetVoyagesFiltered(double? latitudeMin, double? latitudeMax, double? longitudeMin,
        double? longitudeMax, int pageNumber, int pageSize);

    Task<PagedList<VoyageDto>> GetVoyagesByVoyagerUserIdAsync(Guid voyagerUserId, int pageNumber, int pageSize);

    Task<VoyageDto?> GetVoyageByIdAsync(Guid voyageId);

    Task<(Voyage Voyage, List<string> VoyageUploadUrls, List<StopUploadUrlsDto> StopsUploadUrls)>
        AddVoyageWithMediaAsync(CreateVoyageModel createVoyageModel, Guid userId);
    Task UpdateVoyageAsync(Guid voyageId, UpdateVoyageModel updateVoyageModel);
    Task DeleteVoyageAsync(Guid voyageId);
}