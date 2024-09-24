using Mirai.Domain.Common;
using Mirai.Domain.Projects;
using Mirai.Domain.WorkItems;

namespace Mirai.Domain.Tags;

// TODO: Refactor the tag to make it belong to a project and not be global.
public class Tag : Entity
{
    public string Name { get; private set; } = null!;
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;
    public ICollection<WorkItem> WorkItems { get; private set; } = [];

    public Tag(string name)
    {
        Name = name;
    }

    private Tag()
    {
    }
}