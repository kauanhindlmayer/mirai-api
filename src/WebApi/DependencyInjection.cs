using System.Text.Json.Serialization;
using WebApi.Hubs;
using WebApi.Middlewares;
using WebApi.OpenApi;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.ConfigureOptions<ConfigureSwaggerGenOptions>();
        services.ConfigureOptions<ConfigureSwaggerUIOptions>();
        services.AddProblemDetails();
        services.AddSignalR()
            .AddJsonProtocol(options =>
                options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

        return services;
    }

    public static void UsePresentation(this WebApplication app)
    {
        app.MapHub<RetrospectiveHub>("/hubs/retrospective");
        app.UseMiddleware<RequestContextLoggingMiddleware>();
    }
}