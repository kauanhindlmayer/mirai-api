using System.Net.Http.Json;
using Application.Common.Interfaces.Services;
using Contracts.Language;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

internal sealed class LanguageService : ILanguageService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LanguageService> _logger;

    public LanguageService(
        HttpClient httpClient,
        ILogger<LanguageService> logger)
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
                code: "LanguageService.EmbeddingException",
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
                code: "LanguageService.SummarizationException",
                description: "Unexpected error occurred during summarization.");
        }
    }
}