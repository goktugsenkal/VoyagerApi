namespace Core.Dtos.Chat;

public class MessageDto
{
    public Guid ServerId { get; set; }
    public DateTime TimeStamp { get; set; }
    public Guid UserId { get; set; }
    public string Text { get; set; }
    public bool ContainsVoyage { get; set; }
}