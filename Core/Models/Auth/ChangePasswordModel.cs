namespace Core.Models;

public class ChangePasswordModel
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}
