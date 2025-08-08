namespace Presentation.Controllers.Retrospectives;

/// <summary>
/// Request to create a new column in a retrospective.
/// </summary>
/// <param name="Title">The title of the column.</param>
public sealed record CreateColumnRequest(string Title);