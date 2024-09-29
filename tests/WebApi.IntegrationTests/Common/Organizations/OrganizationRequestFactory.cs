using Contracts.Organizations;

namespace WebApi.IntegrationTests.Common.Organizations;

public static class OrganizationRequestFactory
{
    public static CreateOrganizationRequest CreateCreateOrganizationRequest(
        string name = Constants.Organization.Name,
        string description = Constants.Organization.Description)
    {
        return new(name, description);
    }

    public static UpdateOrganizationRequest CreateUpdateOrganizationRequest(
        string name = Constants.Organization.UpdatedName,
        string description = Constants.Organization.UpdatedDescription)
    {
        return new(name, description);
    }
}