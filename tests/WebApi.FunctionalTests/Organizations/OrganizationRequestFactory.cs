using Contracts.Organizations;

namespace WebApi.FunctionalTests.Organizations;

public static class OrganizationRequestFactory
{
    public static CreateOrganizationRequest CreateCreateOrganizationRequest(
        string name = "Organization Name",
        string description = "Organization Description")
    {
        return new CreateOrganizationRequest
        {
            Name = name,
            Description = description,
        };
    }

    public static UpdateOrganizationRequest CreateUpdateOrganizationRequest(
        string name = "Updated Organization Name",
        string description = "Updated Organization Description")
    {
        return new UpdateOrganizationRequest
        {
            Name = name,
            Description = description,
        };
    }
}