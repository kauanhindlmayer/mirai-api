using Mirai.Contracts.Organizations;

namespace Mirai.Api.IntegrationTests.Common.Organizations;

public static class OrganizationRequestFactory
{
    public static CreateOrganizationRequest CreateOrganizationRequest(
        string name = Constants.Organization.Name,
        string description = Constants.Organization.Description)
    {
        return new(name, description);
    }
}