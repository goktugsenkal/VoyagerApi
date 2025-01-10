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
    
    public int ExpectedPrice { get; set; }
    public Currency Currency { get; set; }
}