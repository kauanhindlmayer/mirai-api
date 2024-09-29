using Domain.Common;
using Domain.Projects;
using Domain.WorkItems;

namespace Domain.Tags;

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

    public void UpdateName(string name)
    {
        Name = name;
    }
}