using Domain.Organizations;
using TestCommon.TestConstants;

namespace TestCommon.Organizations;

public static class OrganizationFactory
{
    public const string Name = Constants.Organization.Name;
    public const string Description = Constants.Organization.Description;

    public static Organization CreateOrganization(
        string name = Constants.Organization.Name,
        string description = Constants.Organization.Description)
    {
        return new Organization(name, description);
    }
}