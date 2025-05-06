namespace Application.WikiPages.Queries.GetWikiPage;

public sealed class WikiPageResponse
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public required string Title { get; init; }
    public required string Content { get; init; }
    public AuthorResponse Author { get; init; } = null!;
    public IEnumerable<WikiPageCommentResponse> Comments { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed class WikiPageCommentResponse
{
    public Guid Id { get; init; }
    public AuthorResponse Author { get; init; } = null!;
    public required string Content { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed class AuthorResponse
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
