namespace Infrastructure.Integrations.GitHub;

/// <summary>
/// Configuration for Mirai's GitHub App integration (repository/PR read access
/// and webhooks). Deliberately a distinct config section from
/// <c>GitHubIdentityProviderOptions</c> ("GitHub"), which backs the unrelated
/// Keycloak-brokered "Sign in with GitHub" OAuth App.
/// </summary>
internal sealed class GitHubAppOptions
{
    public const string SectionName = "GitHubApp";

    public string AppId { get; init; } = string.Empty;
    public string PrivateKey { get; init; } = string.Empty;
    public string WebhookSecret { get; init; } = string.Empty;
    public string ClientId { get; init; } = string.Empty;
    public string InstallUrlSlug { get; init; } = string.Empty;
}
