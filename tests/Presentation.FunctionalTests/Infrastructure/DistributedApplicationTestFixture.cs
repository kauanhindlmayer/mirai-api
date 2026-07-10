using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Users.Queries.LoginUser;
using Aspire.Hosting;
using Aspire.Hosting.Postgres;
using Aspire.Hosting.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Presentation.Controllers.Users;
using Presentation.FunctionalTests.Infrastructure;
using Presentation.FunctionalTests.Users;

[assembly: AssemblyFixture(typeof(DistributedApplicationTestFixture))]

namespace Presentation.FunctionalTests.Infrastructure;

public sealed class DistributedApplicationTestFixture : IAsyncLifetime
{
    private const string ApiResourceName = "mirai-api";

    private DistributedApplication? _app;
    private HttpClient? _httpClient;
    private HttpClient? _unauthenticatedHttpClient;
    private HttpClient? _secondaryHttpClient;

    public HttpClient HttpClient => _httpClient
        ?? throw new InvalidOperationException("HttpClient is not initialized.");

    public HttpClient UnauthenticatedHttpClient => _unauthenticatedHttpClient
        ?? throw new InvalidOperationException("UnauthenticatedHttpClient is not initialized.");

    /// <summary>
    /// Authenticated as <see cref="UserRequestFactory.SecondaryEmail"/>, a seeded user who only
    /// holds Member/Contributor roles - used to verify that authorization enforcement denies
    /// actions a caller does not have permission for.
    /// </summary>
    public HttpClient SecondaryHttpClient => _secondaryHttpClient
        ?? throw new InvalidOperationException("SecondaryHttpClient is not initialized.");

    public async ValueTask InitializeAsync()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<global::Projects.AppHost>(
                [
                    "DcpPublisher:RandomizePorts=false"
                ]);

        builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        RemoveUnnecessaryResourcesForTesting(builder);

        _app = await builder.BuildAsync();
        await _app.StartAsync();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await _app.ResourceNotifications.WaitForResourceHealthyAsync(
            ApiResourceName,
            cts.Token);

        _httpClient = _app.CreateHttpClient(ApiResourceName, "https");
        _unauthenticatedHttpClient = _app.CreateHttpClient(ApiResourceName, "https");
        _secondaryHttpClient = _app.CreateHttpClient(ApiResourceName, "https");

        var accessToken = await GetAccessToken(_httpClient, UserRequestFactory.CreateLoginUserRequest());
        SetAuthorizationHeader(_httpClient, accessToken);

        var secondaryAccessToken = await GetAccessToken(
            _secondaryHttpClient,
            UserRequestFactory.CreateLoginUserRequest(email: UserRequestFactory.SecondaryEmail));
        SetAuthorizationHeader(_secondaryHttpClient, secondaryAccessToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_app is not null)
        {
            await _app.DisposeAsync();
        }

        _httpClient?.Dispose();
        _unauthenticatedHttpClient?.Dispose();
        _secondaryHttpClient?.Dispose();
    }

    private static async Task<string> GetAccessToken(HttpClient httpClient, LoginUserRequest request)
    {
        var loginResponse = await httpClient.PostAsJsonAsync(Routes.Users.Login, request);

        var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();

        return tokenResponse!.AccessToken;
    }

    private static void SetAuthorizationHeader(HttpClient httpClient, string accessToken)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessToken);
    }

    private static void RemoveUnnecessaryResourcesForTesting(
        IDistributedApplicationTestingBuilder appHost)
    {
        var resourceTypesToRemove = new Type[]
        {
            typeof(PgAdminContainerResource),
            typeof(RedisInsightResource),
            typeof(NodeAppResource),
        };

        var resourcesToRemove = appHost.Resources
            .Where(r => resourceTypesToRemove.Contains(r.GetType()))
            .ToList();

        foreach (var resource in resourcesToRemove)
        {
            appHost.Resources.Remove(resource);
        }
    }
}
