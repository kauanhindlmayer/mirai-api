namespace Contracts.Boards;

/// <summary>
/// Data transfer object for creating a column in a board.
/// </summary>
/// <param name="Name">The name of the column.</param>
/// <param name="WipLimit">The work in progress limit for the column.</param>
/// <param name="DefinitionOfDone">The definition of done for the column.</param>
/// <param name="Position">The position of the column.</param>
public sealed record CreateColumnRequest(
    string Name,
    int? WipLimit,
    string DefinitionOfDone,
    int Position);