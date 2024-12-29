namespace Application.Common.Interfaces.Services;

public interface IBlobService
{
    Task<string> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default);

    Task<FileResponse> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default);
}
