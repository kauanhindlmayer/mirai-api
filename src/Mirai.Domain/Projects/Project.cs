using ErrorOr;
using Mirai.Domain.Common;
using Mirai.Domain.Organizations;
using Mirai.Domain.Teams;
using Mirai.Domain.WikiPages;
using Mirai.Domain.WorkItems;

namespace Mirai.Domain.Projects;

public class Project : Entity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public ICollection<WorkItem> WorkItems { get; private set; } = [];
    public ICollection<WikiPage> WikiPages { get; private set; } = [];
    public ICollection<Team> Teams { get; private set; } = [];

    public Project(string name, string? description, Guid organizationId)
    {
        Name = name;
        Description = description;
        OrganizationId = organizationId;
    }

    private Project()
    {
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
}