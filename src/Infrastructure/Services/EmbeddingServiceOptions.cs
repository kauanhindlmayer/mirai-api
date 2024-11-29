namespace Infrastructure.Services;

internal sealed class EmbeddingServiceOptions
{
    public const string SectionName = "EmbeddingService";
    public string BaseUrl { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
}