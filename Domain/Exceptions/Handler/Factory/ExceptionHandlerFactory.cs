using Domain.Enumerators;
using Domain.Exceptions.Interface;
using Domain.Utils;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Security.Authentication;
using System.Text;

namespace Domain.Exceptions.Handler.Factory;

public static class ExceptionHandlerFactory
{
    /// <summary>
    /// Retorna um manipulador de exceção apropriado com base no tipo da exceção fornecida.
    /// </summary>
    /// <param name="exception">A exceção que será manipulada.</param>
    /// <returns>Uma instância de <see cref="IExceptionHandler"/> correspondente ao tipo da exceção.</returns>
    public static Interface.IExceptionHandler GetHandler(Exception exception)
    {
        return exception switch
        {
            ValidationException _ => new ValidationExceptionHandler(),
            AuthenticationException _ => new AuthenticationExceptionHandler(),
            ForbiddenException _ => new ForbiddenExceptionHandler(),
            BusinessException _ => new BusinessExceptionHandler(),
            MailException _ => new MailExceptionHandler(),
            PersistenceException _ => new PersistenceExceptionHandler(),
            _ => new GenericExceptionHandler()
        };
    }

    /// <summary>
    /// Mapeia uma exceção para um valor correspondente do enum Operation.
    /// </summary>
    /// <param name="exception">A exceção a ser mapeada.</param>
    /// <returns>Operation correspondente a exceção fornecida.</returns>
    public static Operation GetOperationFromException(Exception exception)
    {
        return exception switch
        {
            ValidationException _ => Operation.VALIDATION_EXCEPTION,
            AuthenticationException _ => Operation.AUTHENTICATION_EXCEPTION,
            ForbiddenException _ => Operation.FORBIDDEN_EXCEPTION,
            BusinessException _ => Operation.BUSINESS_EXCEPTION,
            MailException _ => Operation.MAIL_EXCEPTION,
            PersistenceException _ => Operation.PERSISTENCE_EXCEPTION,
            _ => Operation.SERVER_EXCEPTION
        };
    }

    /// <summary>
    /// Gera mensagem de log do request.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="method"></param>
    /// <param name="bodyRequest"></param>
    /// <returns>Mensagem de log de request.</returns>
    public static string GenerateRequestLogMessage(PathString path, string method, string bodyRequest)
    {
        var builder = new StringBuilder();

        builder.Append($"EndPoint: {path.ToString()} ");
        builder.Append($"| Method: {method}");

        if (!string.IsNullOrEmpty(bodyRequest))
            builder.Append($"| HttpRequest: {bodyRequest}");

        return StringUtil.GetMaxLogString(builder.ToString());
    }

    /// <summary>
    /// Gera mensagem de log da response.
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="exceptionName"></param>
    /// <param name="exception"></param>
    /// <param name="stackTrace"></param>
    /// <returns>Mensagem de log de response.</returns>
    public static string GenerateResponseLogMessage(int statusCode, string exceptionName, string exception, string stackTrace)
    {
        var builder = new StringBuilder();

        builder.Append($"StatusCode: {statusCode} ");
        builder.Append($"| ExceptionName: {exceptionName} ");
        builder.Append($"| ExceptionError: {exception}");
        builder.Append($"| StackTrace: {stackTrace}");

        return StringUtil.GetMaxLogString(builder.ToString());
    }
}
