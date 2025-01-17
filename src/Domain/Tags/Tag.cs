using Domain.Common;
using Domain.Projects;
using Domain.WorkItems;

namespace Domain.Tags;

public sealed class Tag : Entity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the color of the tag in hexadecimal format.
    /// </summary>
    public string Color { get; private set; } = string.Empty;
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;
    public ICollection<WorkItem> WorkItems { get; private set; } = [];

    public Tag(string name, string description, string color)
    {
        Name = name;
        Description = description;
        Color = color;
    }

    private Tag()
    {
    }

    public void Update(string name, string description, string color)
    {
        Name = name;
        Description = description;
        Color = color;
    }
}