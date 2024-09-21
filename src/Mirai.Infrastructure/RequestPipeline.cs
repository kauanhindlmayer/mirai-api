using Microsoft.AspNetCore.Builder;
using Mirai.Infrastructure.Common.Middleware;

namespace Mirai.Infrastructure;

public static class RequestPipeline
{
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestLogContextMiddleware>();
        app.UseMiddleware<EventualConsistencyMiddleware>();

        return app;
    }
}