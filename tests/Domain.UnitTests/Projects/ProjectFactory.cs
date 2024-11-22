using Domain.Projects;

namespace Domain.UnitTests.Projects;

public static class ProjectFactory
{
    public static Project CreateProject(
        Guid? organizationId = null,
        string name = "Project Name",
        string description = "Project Description")
    {
        return new(
            name,
            description,
            organizationId ?? Guid.NewGuid());
    }
}