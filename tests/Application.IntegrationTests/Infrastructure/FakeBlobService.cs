using Application.Abstractions.Storage;

namespace Application.IntegrationTests.Infrastructure;

/// <summary>
/// Replaces the real Azure-backed IBlobService in the integration test host, which has no
/// blob storage container available. Tracks deleted ids in-memory so tests can assert on them.
/// </summary>
public sealed class FakeBlobService : IBlobService
{
    public HashSet<Guid> DeletedFileIds { get; } = [];

    public Task<Guid> UploadAsync(
        Stream stream,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Guid.NewGuid());
    }

    public Task<FileResponse> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new FileResponse(new MemoryStream(), "application/octet-stream"));
    }

    public Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        DeletedFileIds.Add(fileId);
        return Task.CompletedTask;
    }
}
