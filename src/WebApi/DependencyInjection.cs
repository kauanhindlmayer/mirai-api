using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.Hubs;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(configuration);
        services.AddProblemDetails();
        services.AddSignalR();
        services.AddCorsPolicy();

        return services;
    }

    public static IServiceCollection AddSwaggerGen(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            var xmlFilePath = GetXmlCommentsPath();

            ConfigureSwaggerDoc(options);
            ConfigureXmlComments(options, xmlFilePath);
            ConfigureSignalRSwagger(options, xmlFilePath);
            ConfigureSecurity(options, configuration);
        });

        return services;
    }

    public static WebApplication UsePresentation(this WebApplication app)
    {
        app.UseCors();
        app.MapHub<RetrospectiveHub>("/hubs/retrospective");
        return app;
    }

    private static void AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options => options.AddDefaultPolicy(builder =>
            builder.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()));
    }

    private static string GetXmlCommentsPath()
    {
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        return Path.Combine(AppContext.BaseDirectory, xmlFilename);
    }

    private static void ConfigureSwaggerDoc(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Mirai",
            Version = "v1",
            Description = "Mirai (Japanese word for \"future\") is a web-based project management tool that aims to help teams collaborate and manage their projects more effectively.",
        });
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

    private static void ConfigureSecurity(SwaggerGenOptions options, IConfiguration configuration)
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
}