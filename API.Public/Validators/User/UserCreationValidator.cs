using API.Public.DTOs;
using API.Public.Validators._Base;
using Domain.Utils;
using FluentValidation;

namespace API.Public.Validators;

public class UserCreationValidator : BaseValidator<PrivateUserDTO>
{
    public UserCreationValidator()
    {
        RuleFor(m => m.Name)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL")
            .Length(3, 100).WithMessage("INVALID_LENGHT");

        RuleFor(c => c.Email)
            .EmailAddress()
            .WithMessage("INVALID_EMAIL");

        RuleFor(c => c.Cellphone)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .Length(10, 16).WithMessage("INVALID_LENGHT")
            .Must(StringUtil.IsValidCellphone).WithMessage("INVALID_LENGHT");

        RuleFor(m => m.Document)
                .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
                .NotNull().WithMessage("CANNOT_BE_NULL")
                .Must(StringUtil.IsValidCPF).WithMessage("INVALID_DOCUMENT");

        RuleFor(m => m.Password)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL")
            .Must(SecurityUtil.GetPasswordStrength).WithMessage("INVALID_PASSWORD");
    }
}
