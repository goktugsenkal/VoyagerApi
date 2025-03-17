using Core.Enums;

namespace Core.Models;

public class CreateStopModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public int DistanceToNextStop { get; set; }
    public int ArrivalTimeToNextStop { get; set; }
    public TransportationType TransportationTypeToNextStop { get; set; }
    public int ImageCount { get; set; }
    
    public bool IsFocalPoint { get; set; }
    
    // should this be removed from here??
    //public string ImageUrl { get; set; } 
}