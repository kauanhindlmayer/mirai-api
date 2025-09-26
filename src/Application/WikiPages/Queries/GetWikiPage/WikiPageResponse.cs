namespace Application.WikiPages.Queries.GetWikiPage;

public sealed class WikiPageResponse
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public required string Title { get; init; }
    public required string Content { get; init; }
    public AuthorResponse Author { get; init; } = null!;
    public IEnumerable<WikiPageCommentResponse> Comments { get; init; } = [];
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed class WikiPageCommentResponse
{
    public Guid Id { get; init; }
    public AuthorResponse Author { get; init; } = null!;
    public required string Content { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed class AuthorResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? ImageUrl { get; init; }
}
