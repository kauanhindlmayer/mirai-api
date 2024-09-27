using Mirai.Contracts.WorkItems;

namespace Mirai.Contracts.Boards;

public record BoardResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    string Description,
    IEnumerable<BoardColumnResponse> Columns);

public record BoardColumnResponse(
    Guid Id,
    Guid BoardId,
    string Name,
    int Position,
    IEnumerable<BoardCardResponse> Cards);

public record BoardCardResponse(
    Guid Id,
    Guid BoardColumnId,
    WorkItemResponse WorkItem,
    int Position,
    DateTime CreatedAt,
    DateTime? UpdatedAt);