namespace Infrastructure.Settings;

internal sealed class NlpServiceOptions
{
    public const string SectionName = "NlpService";
    public required string BaseUrl { get; init; }
    public required string ApiKey { get; init; }
}