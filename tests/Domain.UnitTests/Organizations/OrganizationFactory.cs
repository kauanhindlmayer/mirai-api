using Domain.Organizations;

namespace Domain.UnitTests.Organizations;

public static class OrganizationFactory
{
    public const string Name = "Organization";
    public const string Description = "Description";

    public static Organization CreateOrganization(
        string name = Name,
        string description = Description)
    {
        return new(name, description);
    }
}