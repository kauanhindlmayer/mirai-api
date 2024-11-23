namespace Infrastructure.Authentication;

internal sealed class AuthenticationOptions
{
    public const string SectionName = "Authentication";
    public string Audience { get; init; } = string.Empty;
    public string MetadataAddress { get; set; } = string.Empty;
    public bool RequireHttpsMetadata { get; init; }
    public string ValidIssuer { get; set; } = string.Empty;
}