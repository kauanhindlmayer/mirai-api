using System.Net.Http.Json;
using Application.Common.Interfaces.Services;
using Domain.Users;
using ErrorOr;
using Infrastructure.Authentication.Models;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication;

internal sealed class JwtService : IJwtService
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _keycloakOptions;
    private readonly ILogger<JwtService> _logger;

    public JwtService(
        HttpClient httpClient,
        IOptions<KeycloakOptions> keycloakOptions,
        ILogger<JwtService> logger)
    {
        _httpClient = httpClient;
        _keycloakOptions = keycloakOptions.Value;
        _logger = logger;
    }

    public async Task<ErrorOr<string>> GetAccessTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var authRequestParameters = new KeyValuePair<string, string>[]
            {
                new("client_id", _keycloakOptions.AuthClientId),
                new("client_secret", _keycloakOptions.AuthClientSecret),
                new("scope", "openid email"),
                new("grant_type", "password"),
                new("username", email),
                new("password", password),
            };

            var authorizationRequestContent = new FormUrlEncodedContent(authRequestParameters);

            var response = await _httpClient.PostAsync(
                string.Empty,
                authorizationRequestContent,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var authorizationToken = await response.Content.ReadFromJsonAsync<AuthorizationToken>(cancellationToken);

            if (authorizationToken is null)
            {
                return UserErrors.InvalidCredentials;
            }

            return authorizationToken.AccessToken;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error while requesting access token from Keycloak.");
            return UserErrors.InvalidCredentials;
        }
    }
}