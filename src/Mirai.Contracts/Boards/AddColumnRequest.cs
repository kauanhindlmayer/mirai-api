namespace Mirai.Contracts.Boards;

public record AddColumnRequest(
    Guid BoardId,
    string Name,
    int WipLimit,
    string DefinitionOfDone);