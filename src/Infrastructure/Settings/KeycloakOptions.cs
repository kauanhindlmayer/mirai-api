namespace Infrastructure.Settings;

internal sealed class KeycloakOptions
{
    public const string SectionName = "Keycloak";
    public required string AdminUrl { get; init; }
    public required string TokenUrl { get; init; }
    public required string AdminClientId { get; init; }
    public required string AdminClientSecret { get; init; }
    public required string AuthClientId { get; init; }
    public required string AuthClientSecret { get; init; }
}