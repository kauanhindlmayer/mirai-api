using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication;

internal sealed class JwtBearerOptionsSetup(IOptions<AuthenticationOptions> authenticationOptions)
    : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AuthenticationOptions _authenticationOptions = authenticationOptions.Value;

    public void Configure(JwtBearerOptions options)
    {
        options.Audience = _authenticationOptions.Audience;
        options.MetadataAddress = _authenticationOptions.MetadataAddress;
        options.RequireHttpsMetadata = _authenticationOptions.RequireHttpsMetadata;
        options.TokenValidationParameters.ValidIssuer = _authenticationOptions.ValidIssuer;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}