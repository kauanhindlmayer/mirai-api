namespace Contracts.Boards;

public sealed record CreateColumnRequest
{
    /// <summary>
    /// The name of the column.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The work in progress limit for the column.
    /// </summary>
    public int? WipLimit { get; init; }

    /// <summary>
    /// The definition of done for the column.
    /// </summary>
    public string DefinitionOfDone { get; init; } = string.Empty;

    /// <summary>
    /// The position of the column.
    /// </summary>
    public required int Position { get; init; }
}
