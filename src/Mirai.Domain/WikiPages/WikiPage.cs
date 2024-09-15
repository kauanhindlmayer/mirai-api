using System.Collections;
using Mirai.Domain.Common;

namespace Mirai.Domain.WikiPages;

public class WikiPage : Entity
{
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public Guid? ParentWikiPageId { get; private set; }
    public WikiPage? ParentWikiPage { get; private set; }
    public ICollection<WikiPage> SubWikiPages { get; private set; } = [];
    public ICollection<WikiPageComment> Comments { get; private set; } = [];

    public WikiPage(Guid projectId, string title, string content)
    {
        ProjectId = projectId;
        Title = title;
        Content = content;
    }

    private WikiPage()
    {
    }
}