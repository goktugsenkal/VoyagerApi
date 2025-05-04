namespace Core.Models.Chat;

public class SignUpForChatModel
{
    public string DisplayName { get; set; } = string.Empty;
    public string StatusMessage { get; set; } = string.Empty;
    public bool AllowDMs { get; set; }
    public bool ShowLastSeen { get; set; }
    public bool ShowOnline { get; set; }
}