namespace Application.WikiPages.Queries.ListWikiPages;

public sealed class WikiPageBriefResponse
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public int Position { get; init; }
    public IEnumerable<WikiPageBriefResponse>? SubPages { get; init; } = null;
}