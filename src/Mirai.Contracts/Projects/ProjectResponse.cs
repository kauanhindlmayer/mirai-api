namespace Mirai.Contracts.Projects;

public record ProjectResponse(
    Guid Id,
    string Name,
    string? Description,
    Guid OrganizationId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);