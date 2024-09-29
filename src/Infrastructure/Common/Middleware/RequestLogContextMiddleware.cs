using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Infrastructure.Common.Middleware;

public class RequestLogContextMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        using (LogContext.PushProperty("CorrelationId", context.TraceIdentifier))
        {
            await _next(context);
        }
    }
}