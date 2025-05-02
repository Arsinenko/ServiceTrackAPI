using System.ComponentModel.DataAnnotations;

namespace AuthApp.application.DTOs;

public class RegisterUserDto
{
    [Required(ErrorMessage = "FirstName is required")]
    public required string FirstName { get; set; }
    
    [Required(ErrorMessage = "LastName is required")]
    public required string LastName { get; set; }
    
    [Required(ErrorMessage = "Email is requered")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at list of 8 characters long")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character")]
    public required string Password { get; set; }
    
    public Guid RoleId { get; set; }
}