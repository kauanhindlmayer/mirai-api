using HealthChecks.UI.Client;
using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Infrastructure;

public static class RequestPipeline
{
    public static IApplicationBuilder UseInfrastructure(this WebApplication app)
    {
        app.UseMiddleware<RequestContextLoggingMiddleware>();
        app.ConfigureHealthChecks();

        return app;
    }

    private static void ConfigureHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        });
    }
}