using Application.Common.Interfaces.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Infrastructure.Services;

internal sealed class BlobService(BlobServiceClient blobServiceClient)
    : IBlobService
{
    private const string ContainerName = "files";

    public async Task<string> UploadAsync(
        Stream stream,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        var fileId = Guid.NewGuid();
        var blobClient = containerClient.GetBlobClient(fileId.ToString());
        await blobClient.UploadAsync(
            stream,
            new BlobHttpHeaders { ContentType = contentType },
            cancellationToken: cancellationToken);
        return blobClient.Uri.ToString();
    }

    public Task DeleteAsync(
        Guid fileId,
        CancellationToken cancellationToken = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        var blobClient = containerClient.GetBlobClient(fileId.ToString());
        return blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<FileResponse> DownloadAsync(
        Guid fileId,
        CancellationToken cancellationToken = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        var blobClient = containerClient.GetBlobClient(fileId.ToString());
        var response = await blobClient.DownloadContentAsync(cancellationToken: cancellationToken);
        return new FileResponse(
            response.Value.Content.ToStream(),
            response.Value.Details.ContentType);
    }
}
