using System.Net.Http.Json;
using Application.Common.Interfaces.Services;
using Contracts.Embeddings;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

internal sealed class EmbeddingService : IEmbeddingService
{
    private const string ErrorMessage = "An error occurred while generating the embedding.";
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmbeddingService> _logger;

    public EmbeddingService(
        HttpClient httpClient,
        ILogger<EmbeddingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ErrorOr<float[]>> GenerateEmbeddingAsync(string text)
    {
        try
        {
            var request = new EmbeddingRequest(text);
            var response = await _httpClient.PostAsJsonAsync("/embed", request);
            var embeddingResponse = await response.Content.ReadFromJsonAsync<EmbeddingResponse>();
            return embeddingResponse!.Embedding;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, ErrorMessage);
            return Error.Failure(
                code: "EmbeddingService.Failure",
                description: ErrorMessage);
        }
    }
}