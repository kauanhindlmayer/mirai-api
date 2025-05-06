using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace WebApi.OpenApi;

public sealed class ConfigureSwaggerUIOptions : IConfigureNamedOptions<SwaggerUIOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerUIOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerUIOptions options)
    {
        foreach (ApiVersionDescription description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName);
        }
    }

    public void Configure(string? name, SwaggerUIOptions options)
    {
        Configure(options);
    }
}
