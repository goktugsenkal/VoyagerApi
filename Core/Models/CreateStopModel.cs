using Core.Enums;

namespace Core.Models;

public class CreateStopModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    //todo: convert distance and arrival time to double
    public int DistanceToNextStop { get; set; }
    public int ArrivalTimeToNextStop { get; set; }
    public TransportationType TransportationTypeToNextStop { get; set; }
    public ushort ImageCount { get; set; }

    public ushort OrderIndex { get; set; }
    public bool IsFocalPoint { get; set; }
    
    // should this be removed from here??
    //public string ImageUrl { get; set; } 
}