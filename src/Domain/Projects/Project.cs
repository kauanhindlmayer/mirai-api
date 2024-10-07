using Domain.Boards;
using Domain.Common;
using Domain.Organizations;
using Domain.Tags;
using Domain.Teams;
using Domain.WikiPages;
using Domain.WorkItems;
using ErrorOr;

namespace Domain.Projects;

public class Project : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public Guid OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public ICollection<WorkItem> WorkItems { get; private set; } = [];
    public ICollection<WikiPage> WikiPages { get; private set; } = [];
    public ICollection<Team> Teams { get; private set; } = [];
    public ICollection<Tag> Tags { get; private set; } = [];
    public ICollection<Board> Boards { get; private set; } = [];

    public Project(string name, string description, Guid organizationId)
    {
        Name = name;
        Description = description;
        OrganizationId = organizationId;
    }

    private Project()
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

        WikiPages.Add(wikiPage);
        return Result.Success;
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

    public ErrorOr<Success> RemoveTag(string tagName)
    {
        var tag = Tags.FirstOrDefault(t => t.Name == tagName);
        if (tag is null)
        {
            return TagErrors.NotFound;
        }

        Tags.Remove(tag);
        return Result.Success;
    }
}