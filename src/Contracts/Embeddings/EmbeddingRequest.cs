namespace Contracts.Embeddings;

/// <summary>
/// Data transfer object for requesting an embedding for a given text.
/// </summary>
/// <param name="Text">The text to be embedded.</param>
public sealed record EmbeddingRequest(string Text);