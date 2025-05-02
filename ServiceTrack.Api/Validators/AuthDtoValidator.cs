using AuthApp.application.DTOs;
using FluentValidation;

namespace AuthApp.Api.Validators;

public class AuthDtoValidator : AbstractValidator<LoginUserDto>
{
    public AuthDtoValidator()
    {
        RuleFor(e => e.Email).NotEmpty().WithMessage("Email required.");
        RuleFor(e => e.Password).NotEmpty().WithMessage("Password required.");
    }
}