using Domain.Enumerators;
using FluentValidation;
using FluentValidation.Results;

namespace API.Public.Validators._Base;

public class BaseValidator<T> : AbstractValidator<T>
{
    public async Task ValidateAndThrowAsync(T schema, string? header = null)
    {
        ValidationResult results = await ValidateAsync(schema);

        if (!results.IsValid || schema is null)
            throw new Domain.Exceptions.ValidationException(ValidationErrorMessage.INVALID_SCHEMA, results.Errors);

        //if (header != null && !ValidAccessEmailId(header))
        //    throw new Domain.Exceptions.ValidationException(ValidationErrorMessage.INVALID_HEADER_TOKEN, results.Errors);
    }

    //private bool ValidAccessEmailId(string headerAccess)
    //    => headerAccess == Constant.Settings.Emailsettings.XAccessEmailId;
}
