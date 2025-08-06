namespace Presentation.Controllers.WikiPages;

/// <summary>
/// Request to move a wiki page to a different parent or position.
/// </summary>
/// <param name="TargetParentId">The unique identifier of the target parent page.</param>
/// <param name="TargetPosition">The target position of the page.</param>
public sealed record MoveWikiPageRequest(
    Guid? TargetParentId,
    int TargetPosition);