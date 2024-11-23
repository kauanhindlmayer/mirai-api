namespace Contracts.Boards;

public sealed record BoardSummaryResponse(
    Guid Id,
    string Name);