namespace Contracts.WikiPages;

public sealed record WikiPageSummaryResponse(
    Guid Id,
    string Title,
    List<WikiPageSummaryResponse>? SubPages = null);