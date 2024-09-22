using Mirai.Domain.Common;

namespace Mirai.Domain.WorkItems;

// TODO: Refactor the tag to make it belong to a project and not be global.
public class Tag : Entity
{
    public string Name { get; private set; } = null!;
    public ICollection<WorkItem> WorkItems { get; private set; } = [];

    public Tag(string name)
    {
        Name = name;
    }

    private Tag()
    {
    }
}