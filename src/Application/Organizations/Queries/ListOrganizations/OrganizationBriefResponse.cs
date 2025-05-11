using Application.Common;

namespace Application.Organizations.Queries.ListOrganizations;

public sealed class OrganizationBriefResponse : LinksResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
}