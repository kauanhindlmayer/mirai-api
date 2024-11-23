namespace Contracts.Projects;

public sealed record ProjectResponse(
    Guid Id,
    string Name,
    string? Description,
    Guid OrganizationId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);