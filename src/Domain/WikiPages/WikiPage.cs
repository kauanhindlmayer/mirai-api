using Domain.Common;
using Domain.Users;
using ErrorOr;

namespace Domain.WikiPages;

public sealed class WikiPage : AggregateRoot
{
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public int Position { get; private set; }
    public Guid AuthorId { get; private set; }
    public User Author { get; private set; } = null!;
    public Guid? ParentWikiPageId { get; private set; }
    public WikiPage? ParentWikiPage { get; private set; }
    public List<WikiPage> SubWikiPages { get; private set; } = [];
    public ICollection<WikiPageComment> Comments { get; private set; } = [];
    public List<WikiPageView> Views { get; private set; } = [];

    public WikiPage(
        Guid projectId,
        string title,
        string content,
        Guid authorId,
        Guid? parentWikiPageId = null)
    {
        ProjectId = projectId;
        Title = title;
        Content = content;
        AuthorId = authorId;
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

    public ErrorOr<Success> InsertSubWikiPage(int position, WikiPage subWikiPage)
    {
        if (position < 0 || position > SubWikiPages.Count)
        {
            return WikiPageErrors.InvalidPosition;
        }

        SubWikiPages.Insert(position, subWikiPage);
        return Result.Success;
    }

    public void RemoveParent()
    {
        ParentWikiPage = null;
    }
}