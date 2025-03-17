using Core.Entities;
using Core.Enums;

namespace Core.Models;

public class CreateVoyageModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string LocationName { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; } // todo: change to nullable
    public bool IsCompleted { get; set; }

    public int StopCount { get; set; }

    public Currency Currency { get; set; }
    public int ExpectedPrice { get; set; }
    public int ActualPrice { get; set; } = -1;

    public int ImageCount { get; set; }
    

    // voyage craete model takes in create stop model
    public List<CreateStopModel> Stops { get; set; } = [];
}

public static class VoyageExtensions
{
    public static Voyage ToVoyageFromCreateVoyageModel(this CreateVoyageModel createVoyageModel)
    {
        return new Voyage
        {
            Title = createVoyageModel.Title,
            Description = createVoyageModel.Description,
            LocationName = createVoyageModel.LocationName,
            ExpectedPrice = createVoyageModel.ExpectedPrice,
            Currency = createVoyageModel.Currency,
            StartDate = createVoyageModel.StartDate,
            EndDate = createVoyageModel.EndDate,
            StopCount = createVoyageModel.StopCount,
            IsCompleted = createVoyageModel.IsCompleted,
            CreatedAt = DateTime.UtcNow,
        };
    }
}