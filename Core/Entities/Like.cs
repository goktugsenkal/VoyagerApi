using System.Text.Json.Serialization;
using Core.Enums;

namespace Core.Entities;

/// <summary>
/// 
/// </summary>
public class Like : BaseEntity
{
    // foreign key VoyageId references Voyages table
    public Guid? VoyageId { get; set; }
    
    // foreign key CommentId references Comments table
    public Guid? CommentId { get; set; }
    
    public LikeType LikeType { get; set; }

    // foreign key UserId references Users table
    public Guid VoyagerUserId { get; set; }
    
    // navigation to Voyage
    [JsonIgnore] 
    public Voyage Voyage  { get; set; }
    
    // navigation to Comment
    [JsonIgnore] 
    public Comment Comment { get; set; }

    // navigation to User
    [JsonIgnore] 
    public VoyagerUser VoyagerUser  { get; set; }
}