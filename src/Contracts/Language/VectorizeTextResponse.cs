namespace Contracts.Language;

/// <summary>
/// Response containing the vector representation of the input text.
/// </summary>
/// <param name="Embedding">The embedding vector.</param>
public sealed record VectorizeTextResponse(float[] Embedding);