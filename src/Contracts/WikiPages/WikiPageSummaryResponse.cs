namespace Contracts.WikiPages;

public record WikiPageSummaryResponse(
    Guid Id,
    string Title,
    List<WikiPageSummaryResponse>? SubPages = null);