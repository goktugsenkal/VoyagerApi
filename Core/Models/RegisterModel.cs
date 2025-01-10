using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class RegisterModel
{
    [MinLength(3)]
    public required string FirstName { get; set; }
    [MinLength(3)]
    public required string LastName { get; set; }
    public string Bio { get; set; } = string.Empty;
    
    [MinLength(5)]
    public required string Username { get; set; }
    public string Email { get; set; } = string.Empty;
    [MinLength(6)]
    public required string Password { get; set; }

    public string PhoneNumber { get; set; }
}