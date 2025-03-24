using Core.Enums;

namespace Core.Dtos;

public class StopDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public int DistanceToNext { get; set; }
    public int ArrivalTimeToNext { get; set; }
    public TransportationType TransportationTypeToNextStop { get; set; }
    public bool IsFocalPoint { get; set; }
    public short OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public List<string> ImageUrls { get; set; }
    public short ImageCount { get; set; }
}