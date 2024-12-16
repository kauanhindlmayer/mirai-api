namespace Contracts.Organizations;

public sealed record OrganizationResponse(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);