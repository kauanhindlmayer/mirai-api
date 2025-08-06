namespace Presentation.Controllers.WisdomExtractor;

/// <summary>
/// Request to extract wisdom from a given context.
/// </summary>
/// <param name="Question">
/// A natural‑language question or prompt guiding the extractor on what insight
/// to retrieve (e.g. "Why is this feature important?").
/// </param>
public sealed record ExtractWisdomRequest(string Question);
