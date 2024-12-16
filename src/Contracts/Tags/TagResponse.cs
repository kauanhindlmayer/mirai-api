namespace Contracts.Tags;

public sealed record TagResponse(
    Guid Id,
    string Name,
    DateTime CreatedAt,
    DateTime UpdatedAt);