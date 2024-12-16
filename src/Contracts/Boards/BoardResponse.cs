namespace Contracts.Boards;

public sealed record BoardResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    string Description,
    IEnumerable<BoardColumnResponse> Columns);

public sealed record BoardColumnResponse(
    Guid Id,
    string Name,
    int Position,
    int? WipLimit,
    string DefinitionOfDone,
    IEnumerable<BoardCardResponse> Cards);

public sealed record BoardCardResponse(
    Guid Id,
    int Position,
    DateTime CreatedAt,
    DateTime UpdatedAt);