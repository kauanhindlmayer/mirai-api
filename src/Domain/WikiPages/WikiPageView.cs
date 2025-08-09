using Domain.Users;

namespace Domain.WikiPages;

public sealed class WikiPageView
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public Guid WikiPageId { get; private set; }
    public WikiPage WikiPage { get; private set; } = null!;
    public DateTime ViewedAt { get; private init; } = DateTime.UtcNow;
    public Guid ViewerId { get; private init; }
    public User Viewer { get; private set; } = null!;

    public WikiPageView(Guid wikiPageId, Guid viewerId)
    {
        WikiPageId = wikiPageId;
        ViewerId = viewerId;
    }

    private WikiPageView()
    {
    }
}