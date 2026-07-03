using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Infrastructure.Storage;
using Microsoft.Extensions.Options;

namespace Presentation.Extensions;

public static class BlobStorageExtensions
{
    public static async Task EnsureBlobContainerPublicAccessAsync(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var blobServiceClient = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();
            var blobStorageOptions = scope.ServiceProvider.GetRequiredService<IOptions<BlobStorageOptions>>();
            var containerClient = blobServiceClient.GetBlobContainerClient(blobStorageOptions.Value.ContainerName);
            await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);
            app.Logger.LogInformation("Blob container public access configured successfully");
        }
        catch (Exception exception)
        {
            app.Logger.LogError(exception, "An error occurred while configuring blob container public access");
            throw;
        }
    }
}
