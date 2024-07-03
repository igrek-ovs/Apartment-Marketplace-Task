using Apartment_Marketplace_Task.dto_s;
using FluentValidation;

namespace Apartment_Marketplace_Task.validation;

public class ApartmentDtoValidator : AbstractValidator<ApartmentDto>
{
    public ApartmentDtoValidator()
    {
        RuleFor(ap => ap.Rooms)
            .GreaterThan(0).WithMessage("Number of rooms must be greater than 0");

        RuleFor(ap => ap.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(ap => ap.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(99).WithMessage("Name cannot be longer than 99 characters");

        RuleFor(ap => ap.Description)
            .MaximumLength(999).WithMessage("Description cannot be longer than 999 characters");
    }
}