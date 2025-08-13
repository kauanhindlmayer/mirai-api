using Domain.Organizations;

namespace Domain.UnitTests.Organizations;

public static class OrganizationFactory
{
    public const string Name = "Test Organization";
    public const string Description = "Test Description";

    public static Organization Create(
        string? name = null,
        string? description = null)
    {
        return new(
            name ?? Name,
            description ?? Description);
    }
}