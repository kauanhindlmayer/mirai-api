using ErrorOr;
using Mirai.Domain.Common;
using Mirai.Domain.Organizations;
using Mirai.Domain.WorkItems;

namespace Mirai.Domain.Projects;

public class Project : Entity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public ICollection<WorkItem> WorkItems { get; private set; } = [];

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
            return Error.Conflict(description: "Work item with the same title already exists");
        }

        WorkItems.Add(workItem);
        return Result.Success;
    }
}