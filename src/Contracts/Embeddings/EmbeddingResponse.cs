namespace Contracts.Embeddings;

/// <summary>
/// Data transfer object for the response containing an embedding for a given text.
/// </summary>
/// <param name="Embedding">The embedding vector.</param>
public sealed record EmbeddingResponse(float[] Embedding);