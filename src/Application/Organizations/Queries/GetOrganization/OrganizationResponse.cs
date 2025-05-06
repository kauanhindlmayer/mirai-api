using Application.Common;

namespace Application.Organizations.Queries.GetOrganization;

public sealed class OrganizationResponse : LinksResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}