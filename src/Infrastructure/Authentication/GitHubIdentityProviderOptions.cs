namespace Infrastructure.Authentication;

internal sealed class GitHubIdentityProviderOptions
{
    public const string SectionName = "GitHub";
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
}
