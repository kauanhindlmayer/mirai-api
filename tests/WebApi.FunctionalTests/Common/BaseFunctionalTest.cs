using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Users.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApi.FunctionalTests.Users;

namespace WebApi.FunctionalTests.Common;

public abstract class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
{
    protected readonly HttpClient HttpClient;

    protected BaseFunctionalTest(FunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
    }

    protected async Task SetAuthorizationHeaderAsync()
    {
        var accessToken = await GetAccessToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessToken);
    }

    private async Task<string> GetAccessToken()
    {
        var loginResponse = await HttpClient.PostAsJsonAsync(
            "api/users/login",
            UserRequestFactory.CreateLoginUserRequest());

        var accessTokenResponse = await loginResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();

        return accessTokenResponse!.AccessToken;
    }
}
