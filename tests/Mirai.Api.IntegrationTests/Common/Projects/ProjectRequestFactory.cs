using Mirai.Contracts.Organizations;
using Mirai.Contracts.Projects;

namespace Mirai.Api.IntegrationTests.Common.Projects;

public static class ProjectRequestFactory
{
    public static CreateProjectRequest CreateCreateProjectRequest(
        string name = Constants.Organization.Name,
        string description = Constants.Organization.Description,
        Guid? organizationId = null)
    {
        return new(
            Name: name,
            Description: description,
            OrganizationId: organizationId ?? Constants.Organization.Id);
    }
}