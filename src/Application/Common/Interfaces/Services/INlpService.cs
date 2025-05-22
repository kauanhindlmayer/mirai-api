using ErrorOr;

namespace Application.Common.Interfaces.Services;

public interface INlpService
{
    Task<ErrorOr<float[]>> GenerateEmbeddingVectorAsync(string text);
    Task<ErrorOr<string>> SummarizeTextAsync(string text);
}