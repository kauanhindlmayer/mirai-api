using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.Hubs;
using WebApi.OpenApi;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddProblemDetails();
        services.AddSignalR();
        services.AddCorsPolicy();

        return services;
    }

    public static IServiceCollection AddSwaggerGen(this IServiceCollection services)
    {
        services.ConfigureOptions<ConfigureSwaggerOptions>();
        services.AddSwaggerGen(options =>
        {
            var xmlFilePath = GetXmlCommentsPath();
            ConfigureXmlComments(options, xmlFilePath);
            ConfigureSignalRSwagger(options, xmlFilePath);
            ConfigureSecurity(options);
        });

        return services;
    }

    public static WebApplication UsePresentation(this WebApplication app)
    {
        app.UseCors();
        app.MapHub<RetrospectiveHub>("/hubs/retrospective");

        return app;
    }

    private static string GetXmlCommentsPath()
    {
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        return Path.Combine(AppContext.BaseDirectory, xmlFilename);
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
                .AllowAnyHeader()));
    }
}