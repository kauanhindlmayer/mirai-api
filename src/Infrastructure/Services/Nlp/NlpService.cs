using System.Net.Http.Json;
using Application.Common.Interfaces.Services;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Nlp;

internal sealed class NlpService : INlpService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NlpService> _logger;

    public NlpService(
        HttpClient httpClient,
        ILogger<NlpService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ErrorOr<float[]>> GenerateEmbeddingVectorAsync(string text)
    {
        try
        {
            var request = new VectorizeTextRequest(text);
            var response = await _httpClient.PostAsJsonAsync("/embed", request);
            var embeddingResponse = await response.Content.ReadFromJsonAsync<VectorizeTextResponse>();
            return embeddingResponse!.Embedding;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while generating an embedding.");
            return Error.Failure(
                code: "NlpService.EmbeddingException",
                description: "Unexpected error occurred during embedding generation.");
        }
    }

    public async Task<ErrorOr<string>> SummarizeTextAsync(string text)
    {
        try
        {
            var request = new SummarizeTextRequest(text);
            var response = await _httpClient.PostAsJsonAsync("/summarize", request);
            var summaryResponse = await response.Content.ReadFromJsonAsync<SummarizeTextResponse>();
            return summaryResponse!.Summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while summarizing text.");
            return Error.Failure(
                code: "NlpService.SummarizationException",
                description: "Unexpected error occurred during summarization.");
        }
    }
}