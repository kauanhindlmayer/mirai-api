using Domain.Common;

namespace Domain.WikiPages;

public class WikiPage : AggregateRoot
{
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public int Position { get; private set; }
    public Guid? ParentWikiPageId { get; private set; }
    public WikiPage? ParentWikiPage { get; private set; }
    public List<WikiPage> SubWikiPages { get; private set; } = [];
    public ICollection<WikiPageComment> Comments { get; private set; } = [];

    public WikiPage(Guid projectId, string title, string content, Guid? parentWikiPageId)
    {
        ProjectId = projectId;
        Title = title;
        Content = content;
        ParentWikiPageId = parentWikiPageId;
    }

    private WikiPage()
    {
    }

    public void Update(string title, string content)
    {
        Title = title;
        Content = content;
    }

    public void UpdatePosition(int position)
    {
        Position = position;
    }

    public void AddComment(WikiPageComment comment)
    {
        Comments.Add(comment);
    }

    public void RemoveComment(WikiPageComment comment)
    {
        Comments.Remove(comment);
    }

    public void InsertSubWikiPage(int position, WikiPage subWikiPage)
    {
        SubWikiPages.Insert(position, subWikiPage);
    }

    public void RemoveParent()
    {
        ParentWikiPage = null;
    }
}