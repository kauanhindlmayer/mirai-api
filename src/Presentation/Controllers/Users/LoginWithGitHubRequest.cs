namespace Presentation.Controllers.Users;

/// <summary>
/// Request to log in a user with GitHub.
/// </summary>
/// <param name="Code">The authorization code returned by Keycloak.</param>
/// <param name="RedirectUri">The redirect URI used in the authorization request.</param>
public sealed record LoginWithGitHubRequest(
    string Code,
    string RedirectUri);
