using Serilog.Context;

namespace WebApi.Middlewares;

internal sealed class RequestContextLoggingMiddleware
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next;

    public RequestContextLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        using (LogContext.PushProperty("CorrelationId", GetCorrelationId(httpContext)))
        {
            await _next(httpContext);
        }
    }

    private static string GetCorrelationId(HttpContext httpContext)
    {
        httpContext.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId);
        return correlationId.FirstOrDefault() ?? httpContext.TraceIdentifier;
    }
}