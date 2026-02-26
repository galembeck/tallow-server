using Domain.Data.Models.Utils;
using Domain.Enumerators;
using Domain.Utils;
using FluentValidation.Results;

namespace Domain.Exceptions;

[Serializable]
public class ValidationException : ApplicationException
{
    public ValidationErrorModel? Error { get; }

    public ValidationException(System.Exception innerExc) : base(innerExc.GetType().Name, innerExc)
    {
    }

    public ValidationException(string innerExc) : base(innerExc)
    {
    }

    public ValidationException(ValidationErrorMessage innerExc) : base(innerExc.ToString())
    {
    }

    public ValidationException(ValidationErrorMessage innerExc, IList<ValidationFailure> errors) : base(innerExc.GetDescription())
    {
        ValidationFailure? error = errors.FirstOrDefault();

        Error = error == null
            ? null
            : new ValidationErrorModel()
            {
                Message = error.ErrorMessage,
                Property = error.PropertyName,
            };
    }
}