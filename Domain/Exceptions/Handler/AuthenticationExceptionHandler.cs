using Domain.Data.Models;
using Domain.Exceptions.Interface;
using Microsoft.AspNetCore.Http;

namespace Domain.Exceptions.Handler;

public record AuthenticationExceptionHandler : IExceptionHandler
{
    public ErrorResponse Handle(Exception exception)
    {
        return new ErrorResponse
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            Message = exception.Message
        };
    }
}
