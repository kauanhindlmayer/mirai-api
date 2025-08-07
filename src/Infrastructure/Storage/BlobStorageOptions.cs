namespace Infrastructure.Storage;

public sealed class BlobStorageOptions
{
    public const string SectionName = "Azure:BlobStorage";
    public required string ConnectionString { get; init; }
    public required string ContainerName { get; init; }
}