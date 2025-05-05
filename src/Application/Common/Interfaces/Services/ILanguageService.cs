using ErrorOr;

namespace Application.Common.Interfaces.Services;

public interface ILanguageService
{
    Task<ErrorOr<float[]>> GenerateEmbeddingVectorAsync(string text);
    Task<ErrorOr<string>> SummarizeTextAsync(string text);
}