using API.Public.DTOs;
using API.Public.Validators._Base;
using Domain.Utils;
using FluentValidation;

namespace API.Public.Validators;

public class AuthenticateValidator : BaseValidator<AuthenticateDTO>
{
    public AuthenticateValidator()
    {
        RuleFor(m => m.Email)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL")
            .EmailAddress().WithMessage("INVALID_EMAIL");

        RuleFor(m => m.Password)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL")
            .Length(6, 30).WithMessage("INVALID_LENGHT");
    }
}