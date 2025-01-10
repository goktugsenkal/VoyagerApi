using System.Text.Json.Serialization;
using Core.Enums;

namespace Core.Entities;

public class Voyage : BaseEntity
{
    public bool IsCompleted { get; set; }
    
    public string Title { get; set; }
    public string Description { get; set; }
    public string LocationName { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public int StopCount { get; set; }

    public Currency Currency { get; set; }
    public int ExpectedPrice { get; set; }
    public int ActualPrice { get; set; }
    
    public string ThumbnailUrl { get; set; }
    public List<string> ImageUrls { get; set; }

    public bool IsArchived { get; set; }
    
    public Guid UserId { get; set; }
    
    // nav
    public ICollection<Stop> Stops { get; set; } // one (voyage) - many (stops)
    [JsonIgnore] // prevent circular serialization
    public VoyagerUser User { get; set; } // many (voyages) - one (user)
}