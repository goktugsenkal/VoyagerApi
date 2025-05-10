namespace Core.Models.Chat;

public class MessageReceivedReceiptModel
{
    public string ReceiptToken { get; set; }
    public Guid MessageId { get; set; }
}