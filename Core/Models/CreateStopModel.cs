using Core.Enums;

namespace Core.Models;

public class CreateStopModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Longitude { get; set; }
    public string Latitude { get; set; }
    public int DistanceToNext { get; set; }
    public int ArrivalTimeToNext { get; set; }
    public TransportationType TransportationTypeToNextStop { get; set; }
    
    // should this be removed from here??
    //public string ImageUrl { get; set; } 
}