using Domain.Projects;

namespace Domain.UnitTests.Projects;

public static class ProjectFactory
{
    public const string Name = "Test Project";
    public const string Description = "Test Description";
    public static readonly Guid OrganizationId = Guid.NewGuid();

    public static Project Create(
        Guid? organizationId = null,
        string? name = null,
        string? description = null)
    {
        return new(
            name ?? Name,
            description ?? Description,
            organizationId ?? OrganizationId);
    }
}