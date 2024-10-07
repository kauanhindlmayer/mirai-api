using System.Net.Http.Json;
using Application.Common.Interfaces;
using Domain.Users;
using Infrastructure.Authentication.Models;

namespace Infrastructure.Authentication;

public class AuthenticationService(HttpClient httpClient)
    : IAuthenticationService
{
    private const string PasswordCredentialType = "password";
    private readonly HttpClient _httpClient = httpClient;

    public async Task<string> RegisterAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default)
    {
        var userRepresentationModel = UserRepresentationModel.FromUser(user);

        userRepresentationModel.Credentials =
        [
            new()
            {
                Value = password,
                Temporary = false,
                Type = PasswordCredentialType,
            }
        ];

        var response = await _httpClient.PostAsJsonAsync(
            "users",
            userRepresentationModel,
            cancellationToken);

        return ExtractIdentityIdFromLocationHeader(response);
    }

    private static string ExtractIdentityIdFromLocationHeader(HttpResponseMessage response)
    {
        const string usersSegmentName = "users/";

        var locationHeader = response.Headers.Location?.PathAndQuery;

        if (locationHeader is null)
        {
            throw new InvalidOperationException("Location header is missing.");
        }

        var userSegmentValueIndex = locationHeader.IndexOf(
            usersSegmentName,
            StringComparison.InvariantCultureIgnoreCase);

        return locationHeader[(userSegmentValueIndex + usersSegmentName.Length)..];
    }
}
