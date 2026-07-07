namespace Application.Abstractions.GitHub;

/// <summary>
/// Builds the GitHub App installation URL. Kept as its own abstraction so the
/// Application layer never needs the App's slug/client id configuration
/// directly (that lives in Infrastructure's <c>GitHubAppOptions</c>).
/// </summary>
public interface IGitHubAppUrlProvider
{
    string BuildInstallUrl(string state);
}
