namespace Application.Organizations.Queries.ListOrganizations;

public sealed class OrganizationBriefResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}