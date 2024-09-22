namespace Mirai.Contracts.Teams;

public record TeamResponse(
    Guid Id,
    string Name,
    Guid ProjectId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);