namespace Infrastructure.Authentication;

public sealed class AuthenticationOptions
{
    public const string SectionName = "Authentication";
    public string Audience { get; init; } = string.Empty;
    public string MetadataAddress { get; init; } = string.Empty;
    public bool RequireHttpsMetadata { get; init; }
    public string ValidIssuer { get; init; } = string.Empty;
}