using Presentation.Controllers.Organizations;

namespace Presentation.FunctionalTests.Organizations;

public static class OrganizationRequestFactory
{
    public static CreateOrganizationRequest CreateCreateOrganizationRequest(
        string? name = null,
        string description = "Organization Description")
    {
        name ??= $"Organization {Guid.NewGuid()}";
        return new CreateOrganizationRequest(name, description);
    }

    public static UpdateOrganizationRequest CreateUpdateOrganizationRequest(
        string name = "Updated Organization Name",
        string description = "Updated Organization Description")
    {
        return new UpdateOrganizationRequest(name, description);
    }
}