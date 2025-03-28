namespace Core.Models;

// Represents the payload for our self-contained verification token.
public class EmailTokenPayload
{
    // The user's unique identifier.
    public Guid UserId { get; set; }
    // The timestamp when the token was generated.
    public DateTime Timestamp { get; set; }
}