using Domain.Common;

namespace Domain.WikiPages;

public class WikiPage : AggregateRoot
{
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public Guid? ParentWikiPageId { get; private set; }
    public WikiPage? ParentWikiPage { get; private set; }
    public ICollection<WikiPage> SubWikiPages { get; private set; } = [];
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

    public void AddComment(WikiPageComment comment)
    {
        Comments.Add(comment);
    }
}