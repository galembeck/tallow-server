using API.Public.DTOs;
using API.Public.Validators._Base;
using FluentValidation;

namespace API.Public.Validators;

public class ProductUpdateValidator : BaseValidator<UpdateProductDTO>
{
    public ProductUpdateValidator()
    {
        RuleFor(m => m.Name)
            .Length(3, 100).WithMessage("INVALID_LENGHT")
            .When(m => m.Name != null);

        RuleFor(m => m.Description)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .When(m => m.Description != null);

        RuleFor(m => m.Price)
            .GreaterThan(0).WithMessage("MUST_BE_GREATER_THAN_ZERO")
            .When(m => m.Price.HasValue);

        RuleFor(m => m.Ingredients)
            .NotEmpty().WithMessage("CANNOT_BE_EMPTY")
            .When(m => m.Ingredients != null);

        RuleFor(m => m.StockAmount)
            .GreaterThanOrEqualTo(0).WithMessage("CANNOT_BE_NEGATIVE")
            .When(m => m.StockAmount.HasValue);

        RuleFor(m => m.Weight)
            .GreaterThan(0).WithMessage("MUST_BE_GREATER_THAN_ZERO")
            .When(m => m.Weight.HasValue);

        RuleFor(m => m.Height)
            .GreaterThan(0).WithMessage("MUST_BE_GREATER_THAN_ZERO")
            .When(m => m.Height.HasValue);

        RuleFor(m => m.Width)
            .GreaterThan(0).WithMessage("MUST_BE_GREATER_THAN_ZERO")
            .When(m => m.Width.HasValue);

        RuleFor(m => m.Length)
            .GreaterThan(0).WithMessage("MUST_BE_GREATER_THAN_ZERO")
            .When(m => m.Length.HasValue);
    }
}
