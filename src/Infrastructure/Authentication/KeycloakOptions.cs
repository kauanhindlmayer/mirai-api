namespace Infrastructure.Authentication;

internal sealed class KeycloakOptions
{
    public const string SectionName = "Keycloak";
    public string AdminUrl { get; set; } = string.Empty;
    public string TokenUrl { get; set; } = string.Empty;
    public string AdminClientId { get; init; } = string.Empty;
    public string AdminClientSecret { get; init; } = string.Empty;
    public string AuthClientId { get; init; } = string.Empty;
    public string AuthClientSecret { get; init; } = string.Empty;
}