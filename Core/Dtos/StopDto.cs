using Core.Enums;

namespace Core.Dtos;

public class StopDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Longitude { get; set; }
    public string Latitude { get; set; }
    public int DistanceToNext { get; set; }
    public int ArrivalTimeToNext { get; set; }
    public TransportationType TransportationTypeToNextStop { get; set; }
    public bool IsFocalPoint { get; set; }
    
    public List<string> ImageUrls { get; set; }
}