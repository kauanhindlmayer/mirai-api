namespace Application.WikiPages.Queries.ListWikiPages;

public sealed class WikiPageBriefResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int Position { get; init; }
    public IEnumerable<WikiPageBriefResponse>? SubPages { get; init; } = null;
}