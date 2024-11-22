using Domain.Projects;

namespace Domain.UnitTests.Projects;

public static class ProjectFactory
{
    public const string Name = "Project Name";
    public const string Description = "Project Description";
    public static readonly Guid OrganizationId = Guid.NewGuid();

    public static Project CreateProject(
        Guid? organizationId = null,
        string name = Name,
        string description = Description)
    {
        return new(
            name,
            description,
            organizationId ?? OrganizationId);
    }
}