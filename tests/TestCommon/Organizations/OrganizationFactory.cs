using Mirai.Domain.Organizations;
using TestCommon.TestConstants;

namespace TestCommon.Organizations;

public static class OrganizationFactory
{
    public static Organization CreateOrganization(
        string name = Constants.Organization.Name,
        string description = Constants.Organization.Description)
    {
        return new(name, description);
    }
}