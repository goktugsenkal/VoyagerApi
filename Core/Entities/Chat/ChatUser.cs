namespace Core.Entities.Chat;

public class ChatUser : UpdatableBaseEntity
{
    public string? DisplayName { get; set; }
    public bool AllowDMs { get; set; }
    public string? StatusMessage { get; set; }
    public DateTime LastSeen { get; set; }
    public bool ShowLastSeen { get; set; }
    public bool ShowOnline { get; set; }
    
    public VoyagerUser User { get; set; }
}