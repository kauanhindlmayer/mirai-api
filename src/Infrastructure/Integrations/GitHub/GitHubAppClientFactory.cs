using Application.Abstractions.Caching;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;

namespace Infrastructure.Integrations.GitHub;

internal sealed class GitHubAppClientFactory : IGitHubAppClientFactory
{
    private const string ProductName = "Mirai";
    private const int LowRateLimitThreshold = 100;
    private static readonly TimeSpan CacheExpiryBuffer = TimeSpan.FromMinutes(5);

    private readonly GitHubAppOptions _options;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GitHubAppClientFactory> _logger;

    public GitHubAppClientFactory(
        IOptions<GitHubAppOptions> options,
        ICacheService cacheService,
        ILogger<GitHubAppClientFactory> logger)
    {
        _options = options.Value;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<IGitHubClient> CreateInstallationClientAsync(
        long installationId,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.GetGitHubInstallationTokenKey(installationId);
        var token = await _cacheService.GetAsync<string>(cacheKey, cancellationToken);

        if (token is null)
        {
            token = await FetchAndCacheInstallationTokenAsync(installationId, cacheKey, cancellationToken);
        }

        return new GitHubClient(new ProductHeaderValue(ProductName))
        {
            Credentials = new Credentials(token),
        };
    }

    private async Task<string> FetchAndCacheInstallationTokenAsync(
        long installationId,
        string cacheKey,
        CancellationToken cancellationToken)
    {
        var appClient = CreateAppClient();
        var accessToken = await appClient.GitHubApps.CreateInstallationToken(installationId);

        LogRateLimitIfLow(appClient);

        var cacheDuration = accessToken.ExpiresAt - DateTimeOffset.UtcNow - CacheExpiryBuffer;
        if (cacheDuration > TimeSpan.Zero)
        {
            await _cacheService.SetAsync(cacheKey, accessToken.Token, cacheDuration, cancellationToken);
        }

        return accessToken.Token;
    }

    private GitHubClient CreateAppClient()
    {
        var jwt = GitHubAppJwtFactory.CreateJwt(_options.AppId, _options.PrivateKey);
        return new GitHubClient(new ProductHeaderValue(ProductName))
        {
            Credentials = new Credentials(jwt, AuthenticationType.Bearer),
        };
    }

    private void LogRateLimitIfLow(GitHubClient appClient)
    {
        var remaining = appClient.GetLastApiInfo()?.RateLimit?.Remaining;
        if (remaining is not null && remaining < LowRateLimitThreshold)
        {
            _logger.LogWarning("GitHub API rate limit is low: {Remaining} requests remaining.", remaining);
        }
    }
}
