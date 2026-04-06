using API.Public.DTOs;
using API.Public.Validators._Base;
using FluentValidation;

namespace API.Public.Validators;

public class AddressUpdateValidator : BaseValidator<UpdateAddressDTO>
{
    public AddressUpdateValidator()
    {
        RuleFor(m => m.AddressTitle)
            .Length(3, 100).WithMessage("INVALID_LENGHT")
            .When(m => m.AddressTitle != null);

        RuleFor(m => m.ReceiverName)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .When(m => m.ReceiverName != null);

        RuleFor(m => m.ReceiverLastname)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .When(m => m.ReceiverLastname != null);

        RuleFor(m => m.ContactCellphone)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .When(m => m.ContactCellphone != null);

        RuleFor(m => m.Zipcode)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .When(m => m.Zipcode != null);

        RuleFor(m => m.Address)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .When(m => m.Address != null);

        RuleFor(m => m.Number)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .When(m => m.Number != null);

        RuleFor(m => m.Complement)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .When(m => m.Complement != null);

        RuleFor(m => m.Neighborhood)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .When(m => m.Neighborhood != null);

        RuleFor(m => m.City)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .When(m => m.City != null);

        RuleFor(m => m.State)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .When(m => m.State != null);
    }
}
