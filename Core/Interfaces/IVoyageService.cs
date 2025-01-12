using Core.Models;

namespace Core.Interfaces;

public interface IVoyageService
{
    Task AddVoyageAsync(CreateVoyageModel createVoyageModel);
}