namespace Contracts.Retrospectives;

public sealed record CreateColumnRequest
{
    /// <summary>
    /// The title of the column.
    /// </summary>
    public string Title { get; init; } = string.Empty;
}