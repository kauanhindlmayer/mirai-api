using Infrastructure.Settings;
using Microsoft.AspNetCore.Builder;

namespace Infrastructure;

public static class RequestPipeline
{
    public static IApplicationBuilder UseInfrastructure(this WebApplication app)
    {
        app.UseCors(CorsOptions.PolicyName);

        return app;
    }
}