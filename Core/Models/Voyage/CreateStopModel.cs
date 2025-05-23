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
    
    // deprecated
    // public int ImageCount { get; set; }
    
    // ["mp4", "jpg", "jpg", "jpg"]
    public List<string> MediaTypes { get; set; } = [];

    public short OrderIndex { get; set; }
    public bool IsFocalPoint { get; set; }
    
    // should this be removed from here??
    //public string ImageUrl { get; set; } 
}