using System.Text.Json.Serialization;
using Core.Dtos;
using Core.Enums;
using Core.Models;

namespace Core.Entities;

public class Stop : UpdatableBaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    
    public int DistanceToNextStop { get; set; }
    public int ArrivalTimeToNextStop { get; set; }
    public TransportationType TransportationTypeToNextStop { get; set; }

    public List<string>? ImageUrls { get; set; } = [];
    public short ImageCount { get; set; }
    
    public bool IsFocalPoint { get; set; } // whether this stop is the focal point of the voyage
    public short OrderIndex { get; set; } // the order in which the stops are visited
    
    public Guid VoyageId { get; set; } // foreign key
    
    [JsonIgnore] // prevent circular serialization
    public Voyage Voyage { get; set; } // nav
}
    

public static class StopMappingExtensions
{
    public static StopDto ToDto(this Stop stop)
    {
        return new StopDto
        {
            Id = stop.Id,
            Name = stop.Name,
            Description = stop.Description,
            Longitude = stop.Longitude,
            Latitude = stop.Latitude,
            DistanceToNext = stop.DistanceToNextStop,
            ArrivalTimeToNext = stop.ArrivalTimeToNextStop,
            TransportationTypeToNextStop = stop.TransportationTypeToNextStop,
            IsFocalPoint = stop.IsFocalPoint,
            OrderIndex = stop.OrderIndex,
            CreatedAt = stop.CreatedAt,
            UpdatedAt = stop.UpdatedAt,
            ImageCount = stop.ImageCount,
            ImageUrls = stop.ImageUrls
        };
    }
    public static Stop ToEntity(this CreateStopModel model)
    {
        return new Stop
        {
            Name = model.Name,
            Description = model.Description,
            Longitude = model.Longitude,
            Latitude = model.Latitude,
            DistanceToNextStop = (int)model.DistanceToNextStop,
            ArrivalTimeToNextStop = (int)model.ArrivalTimeToNextStop,
            TransportationTypeToNextStop = model.TransportationTypeToNextStop,
            IsFocalPoint = model.IsFocalPoint,
            OrderIndex = model.OrderIndex,
            ImageCount = model.ImageCount
        };
    }
    
    public static Stop ToEntity(this StopDto dto)
    {
        return new Stop
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Longitude = dto.Longitude,
            Latitude = dto.Latitude,
            DistanceToNextStop = dto.DistanceToNext,
            ArrivalTimeToNextStop = dto.ArrivalTimeToNext,
            TransportationTypeToNextStop = dto.TransportationTypeToNextStop,
            IsFocalPoint = dto.IsFocalPoint,
            OrderIndex = dto.OrderIndex,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            ImageUrls = dto.ImageUrls
        };
    }

    
    public static Stop CopyWith(
        this Stop stop,
        string? name = null,
        string? description = null,
        double? longitude = null,
        double? latitude = null,
        int? distanceToNext = null,
        int? arrivalTimeToNext = null,
        TransportationType? transportationTypeToNextStop = null,
        List<string>? imageUrls = null,
        short? imageCount = null,
        bool? isFocalPoint = null,
        short? orderIndex = null,
        Guid? voyageId = null,
        Voyage? voyage = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        return new Stop
        {
            Id = stop.Id,
            CreatedAt = createdAt ?? stop.CreatedAt,
            UpdatedAt = updatedAt ?? stop.UpdatedAt,
            Name = name ?? stop.Name,
            Description = description ?? stop.Description,
            Longitude = longitude ?? stop.Longitude,
            Latitude = latitude ?? stop.Latitude,
            DistanceToNextStop = distanceToNext ?? stop.DistanceToNextStop,
            ArrivalTimeToNextStop = arrivalTimeToNext ?? stop.ArrivalTimeToNextStop,
            TransportationTypeToNextStop = transportationTypeToNextStop ?? stop.TransportationTypeToNextStop,
            ImageUrls = imageUrls ?? new List<string>(stop.ImageUrls ?? new List<string>()),
            ImageCount = imageCount ?? stop.ImageCount,
            IsFocalPoint = isFocalPoint ?? stop.IsFocalPoint,
            OrderIndex = orderIndex ?? stop.OrderIndex,
            VoyageId = voyageId ?? stop.VoyageId,
            Voyage = voyage ?? stop.Voyage
        };
    }
}
