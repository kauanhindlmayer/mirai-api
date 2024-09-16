using Mirai.Application.Projects.Commands.CreateProject;
using TestCommon.TestConstants;

namespace TestCommon.Projects;

public static class ProjectCommandFactory
{
    public static CreateProjectCommand CreateCreateProjectCommand(
        string name = Constants.Project.Name,
        string description = Constants.Project.Description,
        Guid? organizationId = null)
    {
        return new(
            Name: name,
            Description: description,
            OrganizationId: organizationId ?? Constants.Project.OrganizationId);
    }
}