namespace Contracts.Retrospectives;

/// <summary>
/// Data transfer object for creating an item in a retrospective board.
/// </summary>
/// <param name="Content">The content of the item.</param>
public sealed record CreateItemRequest(string Content);