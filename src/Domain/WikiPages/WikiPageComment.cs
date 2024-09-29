using Domain.Common;
using Domain.Users;

namespace Domain.WikiPages;

public class WikiPageComment : Entity
{
    public Guid WikiPageId { get; private set; }
    public WikiPage WikiPage { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string Content { get; private set; } = null!;

    public WikiPageComment(Guid wikiPageId, Guid userId, string content)
    {
        WikiPageId = wikiPageId;
        UserId = userId;
        Content = content;
    }

    private WikiPageComment()
    {
    }
}
