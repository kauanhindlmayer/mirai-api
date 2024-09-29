using Contracts.Projects;

namespace WebApi.IntegrationTests.Common.Projects;

public static class ProjectRequestFactory
{
    public static CreateProjectRequest CreateCreateProjectRequest(
        string name = Constants.Organization.Name,
        string description = Constants.Organization.Description)
    {
        return new(name, description);
    }
}