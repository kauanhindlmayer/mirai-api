using Contracts.Organizations;
using TestCommon.TestConstants;

namespace WebApi.FunctionalTests.Organizations;

public static class OrganizationRequestFactory
{
    public static CreateOrganizationRequest CreateCreateOrganizationRequest(
        string name = Constants.Organization.Name,
        string description = Constants.Organization.Description)
    {
        return new CreateOrganizationRequest(name, description);
    }

    public static UpdateOrganizationRequest CreateUpdateOrganizationRequest(
        string name = Constants.Organization.UpdatedName,
        string description = Constants.Organization.UpdatedDescription)
    {
        return new UpdateOrganizationRequest(name, description);
    }
}