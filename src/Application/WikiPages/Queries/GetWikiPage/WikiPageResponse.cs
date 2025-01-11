namespace Application.WikiPages.Queries.GetWikiPage;

public sealed class WikiPageResponse
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public AuthorResponse Author { get; init; } = null!;
    public IEnumerable<WikiPageCommentResponse> Comments { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed class WikiPageCommentResponse
{
    public Guid Id { get; init; }
    public AuthorResponse Author { get; init; } = null!;
    public string Content { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed class AuthorResponse
{
    public string Name { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
}
