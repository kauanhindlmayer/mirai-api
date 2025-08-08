namespace Infrastructure.Authentication;

internal sealed class AuthenticationOptions
{
    public const string SectionName = "Authentication";
    public required string Audience { get; init; }
    public required string MetadataAddress { get; init; }
    public bool RequireHttpsMetadata { get; init; }
    public required string ValidIssuer { get; init; }
}