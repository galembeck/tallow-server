using API.Public.DTOs;
using API.Public.Validators._Base;
using Domain.Utils;
using FluentValidation;

namespace API.Public.Validators;

public class ProductCreationValidator : BaseValidator<CreateProductDTO>
{
    public ProductCreationValidator()
    {
        RuleFor(m => m.Name)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL")
            .Length(3, 100).WithMessage("INVALID_LENGHT");

        RuleFor(m => m.Description)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL");

        RuleFor(m => m.Price)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL");

        RuleFor(m => m.Ingredients)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL");

        RuleFor(m => m.StockAmount)
            .GreaterThanOrEqualTo(1).WithMessage("GREATER_OR_EQUALS_ONE");

        RuleFor(m => m.Weight)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL");

        RuleFor(m => m.Height)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL");

        RuleFor(m => m.Width)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL");

        RuleFor(m => m.Length)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .NotNull().WithMessage("CANNOT_BE_NULL");
    }
}
