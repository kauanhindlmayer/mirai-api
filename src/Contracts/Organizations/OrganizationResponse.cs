namespace Contracts.Organizations;

public record OrganizationResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt);