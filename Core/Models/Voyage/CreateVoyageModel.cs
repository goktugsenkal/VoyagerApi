using Core.Enums;

namespace Core.Models.Voyage;

public class CreateVoyageModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string LocationName { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; } // todo: change to nullable
    
    public bool IsCompleted { get; set; }
    
    public Currency Currency { get; set; }
    public int ExpectedPrice { get; set; }
    public int ActualPrice { get; set; } = -1;

    // deprecated
    // public int ImageCount { get; set; }
    
    // ["mp4", "jpg", "jpg", "jpg"]
    public List<string> MediaTypes { get; set; } = [];

    // voyage craete model takes in create stop model
    public List<CreateStopModel> Stops { get; set; } = [];
}