namespace Contracts.Boards;

public record AddColumnRequest(
    string Name,
    int WipLimit,
    string DefinitionOfDone);