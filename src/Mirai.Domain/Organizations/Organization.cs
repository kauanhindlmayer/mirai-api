using Mirai.Domain.Common;

namespace Mirai.Domain.Organizations;

public class Organization : Entity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public Organization(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    private Organization()
    {
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}