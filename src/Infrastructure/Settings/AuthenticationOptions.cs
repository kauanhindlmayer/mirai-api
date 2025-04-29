namespace Infrastructure.Settings;

internal sealed class AuthenticationOptions
{
    public const string SectionName = "Authentication";
    public required string Audience { get; init; }
    public required string MetadataAddress { get; set; }
    public bool RequireHttpsMetadata { get; init; }
    public required string ValidIssuer { get; set; }
}