namespace Infrastructure.Settings;

internal sealed class EmbeddingServiceOptions
{
    public const string SectionName = "EmbeddingService";
    public required string BaseUrl { get; init; }
    public required string ApiKey { get; init; }
}