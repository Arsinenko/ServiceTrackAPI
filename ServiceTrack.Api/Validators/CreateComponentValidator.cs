using AuthApp.application.DTOs;
using FluentValidation;

namespace AuthApp.Api.Validators;

public class CreateComponentValidator :AbstractValidator<CreateEquipmentDto>
{
    public CreateComponentValidator()
    {
        RuleFor(e => e.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(e => e.Model).NotEmpty().WithMessage("Model is required");
        RuleFor(e => e.SerialNumber).NotEmpty().WithMessage("SerialNumber is required");
        RuleFor(e => e.Manufacturer).NotEmpty().WithMessage("Manufacturer is required");
        RuleFor(e => e.Quantity).NotEmpty().WithMessage("Quantity is required");
    }
}