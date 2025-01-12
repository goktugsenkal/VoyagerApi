using System.Text.Json.Serialization;

namespace Core.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; }
    
    // foreign key UserId references Users table
    public Guid VoyagerUserId { get; set; }
    
    // foreign key VoyageId references Voyages table
    public Guid VoyageId { get; set; }
    
    // navigation to Voyage
    [JsonIgnore] 
    public Voyage Voyage  { get; set; }
    // navigation to User
    [JsonIgnore] 
    public VoyagerUser VoyagerUser  { get; set; }
}