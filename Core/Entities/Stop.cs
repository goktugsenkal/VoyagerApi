using System.Text.Json.Serialization;
using Core.Enums;

namespace Core.Entities;

public class Stop : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public string Longitude { get; set; }
    public string Latitude { get; set; }
    
    public int DistanceToNext { get; set; }
    public int ArrivalTimeToNext { get; set; }
    public TransportationType TransportationTypeToNextStop { get; set; }

    public List<string>? ImageUrls { get; set; } = [];
    
    public bool IsFocalPoint { get; set; }
    
    public Guid VoyageId { get; set; } // 
    // nav
    [JsonIgnore] // prevent circular serialization
    public Voyage Voyage { get; set; }
}
    
