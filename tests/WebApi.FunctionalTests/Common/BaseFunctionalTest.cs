using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Users.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApi.FunctionalTests.Users;

namespace WebApi.FunctionalTests.Common;

public abstract class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
{
    protected readonly HttpClient _httpClient;

    protected BaseFunctionalTest(FunctionalTestWebAppFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    protected async Task SetAuthorizationHeaderAsync()
    {
        var accessToken = await GetAccessToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessToken);
    }

    private async Task<string> GetAccessToken()
    {
        var loginResponse = await _httpClient.PostAsJsonAsync(
            "api/v1/users/login",
            UserRequestFactory.CreateLoginUserRequest());

        var accessTokenResponse = await loginResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();

        return accessTokenResponse!.AccessToken;
    }
}
