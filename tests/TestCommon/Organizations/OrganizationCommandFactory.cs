using Application.Organizations.Commands.CreateOrganization;
using TestCommon.TestConstants;

namespace TestCommon.Organizations;

public static class OrganizationCommandFactory
{
    public static CreateOrganizationCommand CreateCreateOrganizationCommand(
        string name = Constants.Organization.Name,
        string description = Constants.Organization.Description)
    {
        return new(name, description);
    }
}