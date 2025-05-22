namespace Infrastructure.Services.Nlp;

/// <summary>
/// Request to generate a vector representation (embedding) for a given text.
/// </summary>
/// <param name="Text">The text to vectorize.</param>
public sealed record VectorizeTextRequest(string Text);