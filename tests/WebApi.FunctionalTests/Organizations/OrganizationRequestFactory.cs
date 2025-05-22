using WebApi.Controllers.Organizations;

namespace WebApi.FunctionalTests.Organizations;

public static class OrganizationRequestFactory
{
    public static CreateOrganizationRequest CreateCreateOrganizationRequest(
        string name = "Organization Name",
        string description = "Organization Description")
    {
        return new CreateOrganizationRequest(name, description);
    }

    public static UpdateOrganizationRequest CreateUpdateOrganizationRequest(
        string name = "Updated Organization Name",
        string description = "Updated Organization Description")
    {
        return new UpdateOrganizationRequest(name, description);
    }
}