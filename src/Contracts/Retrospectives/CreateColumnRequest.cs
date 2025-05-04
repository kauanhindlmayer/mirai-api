namespace Contracts.Retrospectives;

/// <summary>
/// Data transfer object for creating a column in a retrospective board.
/// </summary>
/// <param name="Title">The title of the column.</param>
public sealed record CreateColumnRequest(string Title);