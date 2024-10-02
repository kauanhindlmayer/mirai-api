namespace Contracts.Boards;

public record CreateColumnRequest(
    string Name,
    int WipLimit,
    string DefinitionOfDone);