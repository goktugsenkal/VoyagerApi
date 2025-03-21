using System.ComponentModel.DataAnnotations;

namespace Core.Models;

using System.ComponentModel.DataAnnotations;

public class RegisterModel
{
    [Required(ErrorMessage = "First name is required.")]
    [MinLength(3, ErrorMessage = "First name must be at least 3 characters long.")]
    public required string FirstName { get; set; }
    
    [Required(ErrorMessage = "Last name is required.")]
    [MinLength(3, ErrorMessage = "Last name must be at least 3 characters long.")]
    public required string LastName { get; set; }
    
    public string Bio { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Username is required.")]
    [MinLength(5, ErrorMessage = "Username must be at least 5 characters long.")]
    public required string Username { get; set; }
    
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [PasswordStrength(MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public required string Password { get; set; }

    [Phone(ErrorMessage = "Invalid phone number.")]
    public string PhoneNumber { get; set; }
}

public class PasswordStrengthAttribute : ValidationAttribute
{
    public int MinimumLength { get; set; } = 6;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var password = value as string;
        if (string.IsNullOrEmpty(password))
        {
            return new ValidationResult("Password is required.");
        }
        
        if (password.Length < MinimumLength)
        {
            return new ValidationResult($"Password must be at least {MinimumLength} characters long.");
        }
        
        if (!password.Any(char.IsUpper))
        {
            return new ValidationResult("Password must contain at least one uppercase letter.");
        }
        
        if (!password.Any(char.IsLower))
        {
            return new ValidationResult("Password must contain at least one lowercase letter.");
        }
        
        if (!password.Any(char.IsDigit))
        {
            return new ValidationResult("Password must contain at least one digit.");
        }
        
        if (password.All(char.IsLetterOrDigit))
        {
            return new ValidationResult("Password must contain at least one special character.");
        }
        
        return ValidationResult.Success;
    }
}
