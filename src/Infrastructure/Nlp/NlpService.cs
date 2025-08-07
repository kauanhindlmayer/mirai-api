using System.Net.Http.Json;
using Application.Abstractions;
using Domain.Shared;
using ErrorOr;
using Infrastructure.Nlp.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Nlp;

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

    public async Task<ErrorOr<float[]>> GenerateEmbeddingVectorAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new VectorizeTextRequest(text);

            var response = await _httpClient.PostAsJsonAsync(
                "/embed",
                request,
                cancellationToken: cancellationToken);

            var embeddingResponse = await response.Content.ReadFromJsonAsync<VectorizeTextResponse>(
                cancellationToken: cancellationToken);

            return embeddingResponse!.Embedding;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while generating an embedding.");
            return Errors.EmbeddingFailure;
        }
    }

    public async Task<ErrorOr<string>> AnswerQuestionAsync(
        string question,
        string context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new AnswerQuestionRequest(question, context);

            var response = await _httpClient.PostAsJsonAsync(
                "/question-answer",
                request,
                cancellationToken: cancellationToken);

            var answerResponse = await response.Content.ReadFromJsonAsync<AnswerQuestionResponse>(
                cancellationToken: cancellationToken);

            return answerResponse!.Answer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while answering a question.");
            return Errors.AnsweringFailure;
        }
    }
}