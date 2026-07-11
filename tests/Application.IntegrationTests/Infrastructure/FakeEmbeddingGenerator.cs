using Microsoft.Extensions.AI;

namespace Application.IntegrationTests.Infrastructure;

/// <summary>
/// Replaces the real Ollama-backed embedding generator in the integration test host, which has
/// no Ollama instance available. Returns a zero vector of the same dimensionality as
/// <see cref="Domain.WorkItems.WorkItem.SearchVector"/> for every input.
/// </summary>
public sealed class FakeEmbeddingGenerator : IEmbeddingGenerator<string, Embedding<float>>
{
    private const int EmbeddingDimensions = 384;

    public Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(
        IEnumerable<string> values,
        EmbeddingGenerationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var embeddings = values.Select(_ => new Embedding<float>(new float[EmbeddingDimensions]));
        return Task.FromResult(new GeneratedEmbeddings<Embedding<float>>(embeddings));
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        return null;
    }

    public void Dispose()
    {
    }
}
