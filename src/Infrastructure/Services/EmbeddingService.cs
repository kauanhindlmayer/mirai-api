using System.Net.Http.Json;
using Application.Common.Interfaces.Services;
using Contracts.Embeddings;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

internal sealed class EmbeddingService(
    HttpClient httpClient,
    ILogger<EmbeddingService> logger) : IEmbeddingService
{
    public async Task<ErrorOr<float[]>> GenerateEmbeddingAsync(string text)
    {
        try
        {
            var request = new EmbeddingRequest(text);
            var response = await httpClient.PostAsJsonAsync("/embed", request);
            response.EnsureSuccessStatusCode();
            var embeddingResponse = await response.Content.ReadFromJsonAsync<EmbeddingResponse>();
            return embeddingResponse!.Embedding;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An error occurred while calling the embedding service.");
            return Error.Failure(
                code: "EmbeddingService.Failure",
                description: "An error occurred while calling the embedding service.");
        }
    }
}