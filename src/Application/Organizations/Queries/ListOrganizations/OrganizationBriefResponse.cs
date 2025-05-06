namespace Application.Organizations.Queries.ListOrganizations;

public sealed class OrganizationBriefResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
}