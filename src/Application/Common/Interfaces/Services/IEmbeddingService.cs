using ErrorOr;

namespace Application.Common.Interfaces.Services;

public interface IEmbeddingService
{
    Task<ErrorOr<float[]>> GenerateEmbeddingAsync(string text);
}