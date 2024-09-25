namespace Mirai.Contracts.Tags;

public record TagResponse(
    Guid Id,
    string Name,
    DateTime CreatedAt,
    DateTime? UpdatedAt);