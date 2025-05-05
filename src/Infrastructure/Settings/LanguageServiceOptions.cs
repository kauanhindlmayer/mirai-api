namespace Infrastructure.Settings;

internal sealed class LanguageServiceOptions
{
    public const string SectionName = "LanguageService";
    public required string BaseUrl { get; init; }
    public required string ApiKey { get; init; }
}