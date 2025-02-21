using System.Reflection;
using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.Hubs;
using WebApi.Middlewares;
using WebApi.OpenApi;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddProblemDetails();
        services.AddSignalR(options => options.EnableDetailedErrors = true)
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                options.PayloadSerializerOptions.MaxDepth = 64;
            });
        services.AddCorsPolicy();

        return services;
    }

    public static void UsePresentation(this WebApplication app)
    {
        app.UseCors();
        app.MapHub<RetrospectiveHub>("/hubs/retrospective");
        app.UseMiddleware<RequestContextLoggingMiddleware>();
        app.ConfigureHealthChecks();
    }

    private static void AddSwaggerGen(this IServiceCollection services)
    {
        services.ConfigureOptions<ConfigureSwaggerOptions>();
        services.AddSwaggerGen(options =>
        {
            var xmlFilePath = GetXmlCommentsPath();
            ConfigureXmlComments(options, xmlFilePath);
            ConfigureSignalRSwagger(options, xmlFilePath);
            ConfigureXmlComments(options, GetContractsXmlCommentsPath());
            ConfigureSecurity(options);
        });
    }

    private static string GetXmlCommentsPath()
    {
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        return Path.Combine(AppContext.BaseDirectory, xmlFilename);
    }

    private static string GetContractsXmlCommentsPath()
    {
        return Path.Combine(AppContext.BaseDirectory, "Contracts.xml");
    }

    private static void ConfigureXmlComments(SwaggerGenOptions options, string xmlFilePath)
    {
        options.IncludeXmlComments(xmlFilePath);
        options.CustomSchemaIds(type => type.ToString());
    }

    private static void ConfigureSignalRSwagger(SwaggerGenOptions options, string xmlFilePath)
    {
        options.AddSignalRSwaggerGen(swaggerGenOptions =>
        {
            swaggerGenOptions.UseHubXmlCommentsSummaryAsTagDescription = true;
            swaggerGenOptions.UseHubXmlCommentsSummaryAsTag = true;
            swaggerGenOptions.UseXmlComments(xmlFilePath);
        });
    }

    private static void ConfigureSecurity(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                Array.Empty<string>()
            },
        });
    }

    private static void AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options => options.AddDefaultPolicy(builder =>
            builder.WithOrigins("http://localhost:5173")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()));
    }

    private static void ConfigureHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        });
    }
}