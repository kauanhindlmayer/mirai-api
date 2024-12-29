namespace Contracts.WikiPages;

public sealed record WikiPageSummaryResponse(
    Guid Id,
    string Title,
    int Position,
    List<WikiPageSummaryResponse>? SubPages = null);