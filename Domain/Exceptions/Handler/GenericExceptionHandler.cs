using Domain.Constants;
using Domain.Data.Models;
using Domain.Exceptions.Interface;
using Domain.Utils;
using Microsoft.AspNetCore.Http;

namespace Domain.Exceptions.Handler;

public record GenericExceptionHandler : IExceptionHandler
{
    public ErrorResponse Handle(Exception exception)
    {
        var errorResponse = new ErrorResponse
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        if (Constant.Settings.Environment == EnvType.Development)
            errorResponse.Message = exception.Message + ": " + exception.InnerException?.Message;
        else
            errorResponse.Message = "Internal Server Error";

        return errorResponse;
    }
}
