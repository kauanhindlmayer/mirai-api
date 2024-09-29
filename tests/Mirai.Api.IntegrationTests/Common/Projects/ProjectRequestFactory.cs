using Mirai.Contracts.Organizations;
using Mirai.Contracts.Projects;

namespace Mirai.Api.IntegrationTests.Common.Projects;

public static class ProjectRequestFactory
{
    public static CreateProjectRequest CreateCreateProjectRequest(
        string name = Constants.Organization.Name,
        string description = Constants.Organization.Description)
    {
        return new(name, description);
    }
}