using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.OpenApi;

internal sealed class ConfigureSwaggerGenOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
        }

        ConfigureXmlComments(options, GetXmlCommentsPath());
        ConfigureXmlComments(options, GetContractsXmlCommentsPath());
        ConfigureSignalRSwagger(options, GetXmlCommentsPath());
        ConfigureSecurity(options);
    }

    public void Configure(string? name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    private static OpenApiInfo CreateVersionInfo(ApiVersionDescription apiVersionDescription)
    {
        var openApiInfo = new OpenApiInfo
        {
            Title = $"Mirai.Api v{apiVersionDescription.ApiVersion}",
            Version = apiVersionDescription.ApiVersion.ToString(),
            Description = "Mirai (Japanese word for \"future\") is a web-based project management tool that aims to"
                          + " help teams collaborate and manage their projects more effectively.",
        };

        if (apiVersionDescription.IsDeprecated)
        {
            openApiInfo.Description += " This API version has been deprecated.";
        }

        return openApiInfo;
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
}
