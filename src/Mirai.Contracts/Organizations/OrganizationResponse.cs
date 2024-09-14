namespace Mirai.Contracts.Organizations;

public record OrganizationResponse(
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt);