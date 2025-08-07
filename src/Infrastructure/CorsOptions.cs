namespace Infrastructure;

public sealed class CorsOptions
{
    public const string PolicyName = "MiraiCorsPolicy";
    public const string SectionName = "Cors";
    public required string[] AllowedOrigins { get; init; }
}