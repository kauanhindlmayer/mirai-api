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
    public const string ErrorMessage = "An error occurred while generating the embedding.";

    public async Task<ErrorOr<float[]>> GenerateEmbeddingAsync(string text)
    {
        try
        {
            var request = new EmbeddingRequest(text);
            var response = await httpClient.PostAsJsonAsync("/embed", request);
            var embeddingResponse = await response.Content.ReadFromJsonAsync<EmbeddingResponse>();
            return embeddingResponse!.Embedding;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, ErrorMessage);
            return Error.Failure(
                code: "EmbeddingService.Failure",
                description: ErrorMessage);
        }
    }
}