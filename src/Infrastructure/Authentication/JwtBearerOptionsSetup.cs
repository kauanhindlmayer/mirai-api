using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication;

internal sealed class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AuthenticationOptions _authenticationOptions;
    private readonly ILoggerFactory _loggerFactory;

    public JwtBearerOptionsSetup(
        IOptions<AuthenticationOptions> authenticationOptions,
        ILoggerFactory loggerFactory)
    {
        _authenticationOptions = authenticationOptions.Value;
        _loggerFactory = loggerFactory;
    }

    public void Configure(JwtBearerOptions options)
    {
        options.Audience = _authenticationOptions.Audience;
        options.MetadataAddress = _authenticationOptions.MetadataAddress;
        options.RequireHttpsMetadata = _authenticationOptions.RequireHttpsMetadata;
        options.TokenValidationParameters.ValidIssuer = _authenticationOptions.ValidIssuer;

        var logger = _loggerFactory.CreateLogger<JwtBearerOptionsSetup>();

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                logger.LogError(
                    context.Exception,
                    "JWT authentication failed. Error: {Error}",
                    context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                logger.LogInformation(
                    "JWT token validated successfully for user: {User}",
                    context.Principal?.Identity?.Name ?? "Unknown");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                logger.LogWarning(
                    "JWT authentication challenge. Error: {Error}, ErrorDescription: {ErrorDescription}, ErrorUri: {ErrorUri}",
                    context.Error,
                    context.ErrorDescription,
                    context.ErrorUri);
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                var hasAuthHeader = !string.IsNullOrEmpty(authHeader);
                var tokenPreview = hasAuthHeader && authHeader!.Length > 20
                    ? authHeader.Substring(0, 20) + "..."
                    : authHeader ?? "null";

                logger.LogInformation(
                    "JWT message received. Has Authorization header: {HasAuthHeader}, Token preview: {TokenPreview}",
                    hasAuthHeader,
                    tokenPreview);
                return Task.CompletedTask;
            },
        };
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}