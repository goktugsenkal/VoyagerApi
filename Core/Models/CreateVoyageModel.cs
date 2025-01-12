using Core.Entities;
using Core.Enums;

namespace Core.Models;

public class CreateVoyageModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string LocationName { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public int StopCount { get; set; }

    public Currency Currency { get; set; }
    public int ExpectedPrice { get; set; }
    public int ActualPrice { get; set; } = -1;
    
    // probably will have a different endpoint to upload the images
    // so not necessary in the voyage creation endpoint
    //public string ThumbnailUrl { get; set; } = string.Empty;
    //public List<string> ImageUrls { get; set; } = [];

    public ICollection<Stop?> Stops { get; set; } = [];
}