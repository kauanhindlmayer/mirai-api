using Domain.Common;
using ErrorOr;

namespace Domain.WikiPages;

public sealed class WikiPage : AggregateRoot
{
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public int Position { get; private set; }
    public Guid? ParentWikiPageId { get; private set; }
    public WikiPage? ParentWikiPage { get; private set; }
    public List<WikiPage> SubWikiPages { get; private set; } = [];
    public ICollection<WikiPageComment> Comments { get; private set; } = [];

    public WikiPage(
        Guid projectId,
        string title,
        string content,
        Guid? parentWikiPageId)
    {
        ProjectId = projectId;
        Title = title;
        Content = content;
        ParentWikiPageId = parentWikiPageId;
    }

    public WikiPage()
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

    public ErrorOr<Success> RemoveComment(Guid commentId)
    {
        var comment = Comments.FirstOrDefault(c => c.Id == commentId);
        if (comment is null)
        {
            return WikiPageErrors.CommentNotFound;
        }

        Comments.Remove(comment);
        return Result.Success;
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