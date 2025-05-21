namespace Infrastructure.Services.Language;

/// <summary>
/// Request to summarize a given text.
/// </summary>
/// <param name="Text">The input text to be summarized.</param>
public sealed record SummarizeTextRequest(string Text);