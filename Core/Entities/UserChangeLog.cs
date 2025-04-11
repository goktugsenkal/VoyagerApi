namespace Core.Entities;

public class UserChangeLog
{
    public Guid Id { get; set; }
    public Guid VoyagerUserId { get; set; }
    public string ChangedByUsername { get; set; }
    public string FieldName { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }
    public string IPAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime ChangedAt { get; set; }
}