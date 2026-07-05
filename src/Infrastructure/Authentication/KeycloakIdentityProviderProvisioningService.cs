using System.Net;
using System.Net.Http.Json;
using Application.Abstractions.Authentication;
using Infrastructure.Authentication.Models;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication;

internal sealed class KeycloakIdentityProviderProvisioningService : IIdentityProviderProvisioningService
{
    private const string GitHubIdentityProviderAlias = "github";
    private readonly HttpClient _httpClient;
    private readonly GitHubIdentityProviderOptions _gitHubOptions;

    public KeycloakIdentityProviderProvisioningService(
        HttpClient httpClient,
        IOptions<GitHubIdentityProviderOptions> gitHubOptions)
    {
        _httpClient = httpClient;
        _gitHubOptions = gitHubOptions.Value;
    }

    public async Task EnsureGitHubIdentityProviderAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_gitHubOptions.ClientId) || string.IsNullOrEmpty(_gitHubOptions.ClientSecret))
        {
            return;
        }

        var identityProvider = IdentityProviderRepresentationModel.FromClientCredentials(
            _gitHubOptions.ClientId,
            _gitHubOptions.ClientSecret);

        if (!await GitHubIdentityProviderExistsAsync(cancellationToken))
        {
            await _httpClient.PostAsJsonAsync("identity-provider/instances", identityProvider, cancellationToken);
            return;
        }

        await _httpClient.PutAsJsonAsync(
            $"identity-provider/instances/{GitHubIdentityProviderAlias}",
            identityProvider,
            cancellationToken);
    }

    private async Task<bool> GitHubIdentityProviderExistsAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _httpClient.GetAsync(
                $"identity-provider/instances/{GitHubIdentityProviderAlias}",
                cancellationToken);
            return true;
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
