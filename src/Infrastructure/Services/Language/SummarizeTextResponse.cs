namespace Infrastructure.Services.Language;

/// <summary>
/// Response containing the summarized version of the input text.
/// </summary>
/// <param name="Summary">The generated summary.</param>
public sealed record SummarizeTextResponse(string Summary);
