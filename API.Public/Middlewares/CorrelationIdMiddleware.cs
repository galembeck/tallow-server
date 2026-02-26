using API.Public.Resources;
using Microsoft.Extensions.Options;

namespace API.Public.Middlewares;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly CorrelationIdOptions _options;

    public CorrelationIdMiddleware(RequestDelegate next, IOptions<CorrelationIdOptions> options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        _next = next ?? throw new ArgumentNullException(nameof(next));
        _options = options.Value;
    }

    public Task Invoke(HttpContext context)
    {
        context.TraceIdentifier = Guid.NewGuid().ToString();

        if (_options.IncludeInResponse)
            context.Response.Headers.TryAdd(_options.Header, context.TraceIdentifier);

        return _next(context);
    }
}
