namespace Mirai.Contracts.WorkItems;

public record WorkItemResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    string Status,
    string Type,
    DateTime CreatedAt,
    DateTime? UpdatedAt);