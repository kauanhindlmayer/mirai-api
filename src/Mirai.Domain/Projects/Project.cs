using Mirai.Domain.Common;

namespace Mirai.Domain.Projects;

public class Project : Entity
{
    public Guid OrganizationId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public Project(string name, string description, Guid organizationId)
    {
        Name = name;
        Description = description;
        OrganizationId = organizationId;
    }

    private Project()
    {
    }
}