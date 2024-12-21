using Domain.Common;
using Domain.Users;

namespace Domain.WikiPages;

public sealed class WikiPageComment : Entity
{
    public Guid WikiPageId { get; private set; }
    public WikiPage WikiPage { get; private set; } = null!;
    public Guid AuthorId { get; private set; }
    public User Author { get; private set; } = null!;
    public string Content { get; private set; } = null!;

    public WikiPageComment(Guid wikiPageId, Guid authorId, string content)
    {
        WikiPageId = wikiPageId;
        AuthorId = authorId;
        Content = content;
    }

    private WikiPageComment()
    {
    }
}
