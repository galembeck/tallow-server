using API.Public.Resources;
using Domain.Data.Models;
using Domain.Exceptions.Handler.Factory;
using Domain.Utils;
using Microsoft.Extensions.Primitives;

namespace API.Public.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    private static IdentityPrincipal Authenticated => (IdentityPrincipal)Thread.CurrentPrincipal;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            httpContext.Request.EnableBuffering();

            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    /// <summary>
    /// Trata exceções globais da aplicação, gerando uma resposta de erro padronizada.
    /// </summary>
    /// <param name="context">O contexto HTTP da requisição.</param>
    /// <param name="exception">A exceção capturada.</param>
    /// <param name="logService">Serviço utilizado para registrar logs de exceções.</param>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    { 
        ErrorResponse errorResponse = ExceptionHandlerFactory.GetHandler(exception).Handle(exception);

        context.Response.StatusCode = errorResponse.StatusCode;
        context.Response.ContentType = "application/problem+json; charset=utf-8";

        string userId = Authenticated?.User?.Id ?? "EMPTY";
        string correlationId = context.Response.Headers.TryGetValue("X-Correlation-ID", out StringValues correlationIdValue)
            ? correlationIdValue.ToString()
            : "NO_CORRELATION_ID";

        await context.Response.WriteAsync(errorResponse.ToJson());
    }

    #region Private Methods

    /// <summary>
    /// Lê o corpo da requisição HTTP e o retorna como uma string.
    /// </summary>
    /// <param name="request">A requisição HTTP.</param>
    /// <returns>O corpo da requisição como string.</returns>
    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body);
        return await reader.ReadToEndAsync();
    }

    #endregion Private Methods
}
