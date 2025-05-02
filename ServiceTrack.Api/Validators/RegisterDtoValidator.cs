using AuthApp.application.DTOs;
using FluentValidation;

namespace AuthApp.Api.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(e => e.Email).NotEmpty().WithMessage("Email required.")
            .EmailAddress().WithMessage("Invalid email format");
        RuleFor(e => e.Password).NotEmpty().WithMessage("Password required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
        RuleFor(e => e.RoleId).NotEmpty().WithMessage("Role required");
    }
    
}