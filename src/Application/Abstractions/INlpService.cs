using ErrorOr;

namespace Application.Abstractions;

public interface INlpService
{
    Task<ErrorOr<float[]>> GenerateEmbeddingVectorAsync(
        string text,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<string>> AnswerQuestionAsync(
        string question,
        string context,
        CancellationToken cancellationToken = default);
}