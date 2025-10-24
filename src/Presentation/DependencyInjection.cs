using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Presentation.Constants;
using Presentation.Converters;
using Presentation.Hubs;
using Presentation.Middlewares;
using Presentation.OpenApi;

namespace Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers(options => options.ReturnHttpNotAcceptable = true)
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
            });

        services.ConfigureCustomMediaTypes();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.ConfigureOptions<ConfigureSwaggerGenOptions>();
        services.ConfigureOptions<ConfigureSwaggerUIOptions>();

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddSignalR()
            .AddJsonProtocol(config =>
                config.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

        return services;
    }

    public static void UsePresentation(this WebApplication app)
    {
        app.MapHub<RetrospectiveHub>("/hubs/retrospective");
        app.UseMiddleware<RequestContextLoggingMiddleware>();
    }

    private static void ConfigureCustomMediaTypes(this IServiceCollection services)
    {
        services.Configure<MvcOptions>(config =>
        {
            var formatter = config.OutputFormatters
                .OfType<SystemTextJsonOutputFormatter>()
                .FirstOrDefault()!;

            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.JsonV1);
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.JsonV2);
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.HateoasJson);
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.HateoasJsonV1);
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.HateoasJsonV2);
        });
    }
}