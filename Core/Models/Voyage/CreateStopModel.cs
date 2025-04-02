using Core.Enums;

namespace Core.Models;

public class CreateStopModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public double Longitude { get; set; }
    public double Latitude { get; set; }

    public double DistanceToNextStop { get; set; }
    public double ArrivalTimeToNextStop { get; set; }
    public TransportationType TransportationTypeToNextStop { get; set; }
    public short ImageCount { get; set; }

    public short OrderIndex { get; set; }
    public bool IsFocalPoint { get; set; }
    
    // should this be removed from here??
    //public string ImageUrl { get; set; } 
}