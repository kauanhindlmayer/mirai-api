using Domain.Projects;
using Domain.Shared;
using Domain.Users;
using ErrorOr;

namespace Domain.WikiPages;

public sealed class WikiPage : AggregateRoot
{
    public string Title { get; private set; } = null!;
    public string Content { get; private set; } = string.Empty;
    public int Position { get; private set; }
    public Guid AuthorId { get; private set; }
    public User Author { get; private set; } = null!;
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;
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

    public ErrorOr<Success> UpdateComment(Guid commentId, string content, Guid userId)
    {
        var comment = Comments.FirstOrDefault(c => c.Id == commentId);
        if (comment is null)
        {
            return WikiPageErrors.CommentNotFound;
        }

        if (comment.AuthorId != userId)
        {
            return WikiPageErrors.CommentNotOwned;
        }

        comment.UpdateContent(content);
        return Result.Success;
    }

    public ErrorOr<Success> InsertSubWikiPage(int position, WikiPage subWikiPage)
    {
        if (position < 0 || position > SubWikiPages.Count)
        {
            return WikiPageErrors.InvalidPosition;
        }

        ShiftSubWikiPages(position, 1);
        subWikiPage.UpdatePosition(position);
        subWikiPage.SetParent(this);
        SubWikiPages.Insert(position, subWikiPage);
        return Result.Success;
    }

    public void SetParent(WikiPage parentWikiPage)
    {
        ParentWikiPage = parentWikiPage;
        ParentWikiPageId = parentWikiPage.Id;
    }

    public void RemoveParent()
    {
        ParentWikiPage = null;
    }

    private void ShiftSubWikiPages(int fromIndex, int offset)
    {
        foreach (var page in SubWikiPages.Where(p => p.Position >= fromIndex))
        {
            page.UpdatePosition(page.Position + offset);
        }
    }
}