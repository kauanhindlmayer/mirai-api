using Contracts.Projects;
using TestCommon.TestConstants;

namespace WebApi.FunctionalTests.Projects;

public static class ProjectRequestFactory
{
    public static CreateProjectRequest CreateCreateProjectRequest(
        string name = Constants.Organization.Name,
        string description = Constants.Organization.Description)
    {
        return new CreateProjectRequest(name, description);
    }
}