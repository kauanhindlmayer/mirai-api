using Domain.Boards;
using Domain.Common;
using Domain.Organizations;
using Domain.Tags;
using Domain.Teams;
using Domain.WikiPages;
using Domain.WorkItems;
using ErrorOr;

namespace Domain.Projects;

public sealed class Project : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public Guid OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public ICollection<WorkItem> WorkItems { get; private set; } = [];
    public List<WikiPage> WikiPages { get; private set; } = [];
    public ICollection<Team> Teams { get; private set; } = [];
    public ICollection<Tag> Tags { get; private set; } = [];
    public ICollection<Board> Boards { get; private set; } = [];

    public Project(string name, string description, Guid organizationId)
    {
        Name = name;
        Description = description;
        OrganizationId = organizationId;
    }

    public Project()
    {
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public ErrorOr<Success> AddWorkItem(WorkItem workItem)
    {
        if (WorkItems.Any(wi => wi.Title == workItem.Title))
        {
            return ProjectErrors.WorkItemWithSameTitleAlreadyExists;
        }

        WorkItems.Add(workItem);
        return Result.Success;
    }

    public ErrorOr<Success> AddWikiPage(WikiPage wikiPage)
    {
        if (WikiPages.Any(wp => wp.Title == wikiPage.Title))
        {
            return ProjectErrors.WikiPageWithSameTitleAlreadyExists;
        }

        wikiPage.UpdatePosition(WikiPages.Count);
        WikiPages.Add(wikiPage);
        return Result.Success;
    }

    public ErrorOr<Success> MoveWikiPage(Guid wikiPageId, Guid? targetParentId, int targetPosition)
    {
        var wikiPage = WikiPages.FirstOrDefault(wp => wp.Id == wikiPageId);
        if (wikiPage is null)
        {
            return WikiPageErrors.NotFound;
        }

        WikiPages.Remove(wikiPage);

        if (targetParentId is not null)
        {
            var targetParent = WikiPages.FirstOrDefault(wp => wp.Id == targetParentId);
            if (targetParent is null)
            {
                return WikiPageErrors.ParentWikiPageNotFound;
            }

            targetParent.InsertSubWikiPage(targetPosition, wikiPage);
        }
        else
        {
            wikiPage.RemoveParent();
            WikiPages.Insert(targetPosition, wikiPage);
        }

        ReorderWikiPages();

        return Result.Success;
    }

    public void ReorderWikiPages()
    {
        var position = 0;
        foreach (var wikiPage in WikiPages.OrderBy(wp => wp.Position))
        {
            wikiPage.UpdatePosition(position);
            position++;
        }
    }

    public ErrorOr<Success> AddTeam(Team team)
    {
        if (Teams.Any(t => t.Name == team.Name))
        {
            return ProjectErrors.TeamWithSameNameAlreadyExists;
        }

        Teams.Add(team);
        return Result.Success;
    }

    public ErrorOr<Success> AddTag(Tag tag)
    {
        if (Tags.Any(t => t.Name == tag.Name))
        {
            return TagErrors.AlreadyExists;
        }

        Tags.Add(tag);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveTag(Tag tag)
    {
        if (tag.WorkItems.Count > 0)
        {
            return TagErrors.TagHasWorkItems;
        }

        Tags.Remove(tag);
        return Result.Success;
    }
}