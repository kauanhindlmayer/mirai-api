using System.Net.Http.Json;
using Application.Abstractions.Authentication;
using Domain.Users;
using Google.Apis.Auth;
using Infrastructure.Authentication.Models;

namespace Infrastructure.Authentication;

internal sealed class AuthenticationService : IAuthenticationService
{
    private const string PasswordCredentialType = "password";
    private readonly HttpClient _httpClient;

    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> RegisterAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default)
    {
        var userRepresentationModel = UserRepresentationModel.FromUser(user);

        userRepresentationModel.Credentials =
        [
            new CredentialRepresentationModel
            {
                Type = PasswordCredentialType,
                Value = password,
                Temporary = false,
            }
        ];

        var response = await _httpClient.PostAsJsonAsync(
            "users",
            userRepresentationModel,
            cancellationToken);

        return ExtractIdentityIdFromLocationHeader(response);
    }

    public async Task<string> RegisterWithGoogleAsync(
        User user,
        string googleIdToken,
        CancellationToken cancellationToken = default)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(googleIdToken);

        if (!string.Equals(payload.Email, user.Email, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Google email does not match provided email.");
        }

        var userRepresentationModel = UserRepresentationModel.FromUser(user);
        userRepresentationModel.EmailVerified = true;
        userRepresentationModel.Enabled = true;

        var response = await _httpClient.PostAsJsonAsync(
            "users",
            userRepresentationModel,
            cancellationToken);

        var identityId = ExtractIdentityIdFromLocationHeader(response);

        var linkResponse = await _httpClient.PostAsJsonAsync(
            $"users/{identityId}/federated-identity/google",
            new
            {
                identityProvider = "google",
                userId = payload.Subject,
                userName = payload.Email,
            },
            cancellationToken);

        linkResponse.EnsureSuccessStatusCode();

        return identityId;
    }

    private static string ExtractIdentityIdFromLocationHeader(HttpResponseMessage response)
    {
        const string usersSegmentName = "users/";

        var locationHeader = response.Headers.Location?.PathAndQuery;

        if (locationHeader is null)
        {
            throw new InvalidOperationException("Location header is missing.");
        }

        int userSegmentValueIndex = locationHeader.IndexOf(
            usersSegmentName,
            StringComparison.InvariantCultureIgnoreCase);

        string userIdentityId = locationHeader.Substring(
            userSegmentValueIndex + usersSegmentName.Length);

        return userIdentityId;
    }
}
