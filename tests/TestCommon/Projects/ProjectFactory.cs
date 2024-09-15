using Mirai.Domain.Projects;
using TestCommon.TestConstants;

namespace TestCommon.Projects;

public static class ProjectFactory
{
    public static Project CreateProject(
        string name = Constants.Project.Name,
        string description = Constants.Project.Description,
        Guid? organizationId = null)
    {
        return new(
            name: name,
            description: description,
            organizationId: organizationId ?? Constants.Organization.Id);
    }
}