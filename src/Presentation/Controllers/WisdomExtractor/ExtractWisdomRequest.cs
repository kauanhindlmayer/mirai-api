namespace Presentation.Controllers.WisdomExtractor;

/// <summary>
/// Represents a request to the Wisdom Extractor feature, which will search
/// your project’s work items and wiki pages to generate a focused answer and
/// its source excerpts.
/// </summary>
/// <param name="Question">
/// A natural‑language question or prompt guiding the extractor on what insight
/// to retrieve (e.g. "Why is this feature important?").
/// </param>
public sealed record ExtractWisdomRequest(string Question);
