namespace Presentation.Controllers.Retrospectives;

/// <summary>
/// Request to create a new item in a retrospective.
/// </summary>
/// <param name="Content">The content of the item.</param>
public sealed record CreateItemRequest(string Content);