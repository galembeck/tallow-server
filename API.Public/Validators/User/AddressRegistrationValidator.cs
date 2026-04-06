using API.Public.DTOs;
using API.Public.Validators._Base;
using FluentValidation;

namespace API.Public.Validators;

public class AddressRegistrationValidator : BaseValidator<RegisterAddressDTO>
{
    public AddressRegistrationValidator()
    {
        RuleFor(m => m.AddressTitle)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL")
            .Length(3, 100).WithMessage("INVALID_LENGHT");

        RuleFor(m => m.Zipcode)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL")
            .Length(8).WithMessage("INVALID_LENGHT");

        RuleFor(m => m.Address)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL");

        RuleFor(m => m.Number)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL");

        RuleFor(m => m.Neighborhood)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL");

        RuleFor(m => m.City)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL");

        RuleFor(m => m.State)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL");
    }
}
