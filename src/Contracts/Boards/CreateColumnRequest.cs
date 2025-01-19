namespace Contracts.Boards;

public sealed record CreateColumnRequest(
    string Name,
    int WipLimit,
    string DefinitionOfDone,
    int Position);