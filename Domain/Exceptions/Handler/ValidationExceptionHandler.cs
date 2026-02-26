using Domain.Data.Models;
using Microsoft.AspNetCore.Http;

namespace Domain.Exceptions.Handler;

public record ValidationExceptionHandler : Interface.IExceptionHandler
{
    public ErrorResponse Handle(Exception exception)
    {
        return new ErrorResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = exception.Message,
            Error = new ErrorDetail
            {
                Property = ((ValidationException)exception).Error?.Property,
                Message = ((ValidationException)exception).Error?.Message
            },
        };
    }
}