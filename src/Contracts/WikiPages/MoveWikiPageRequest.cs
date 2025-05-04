namespace Contracts.WikiPages;

/// <summary>
/// Data transfer object for moving a wiki page.
/// </summary>
/// <param name="TargetParentId">The unique identifier of the target parent page.</param>
/// <param name="TargetPosition">The target position of the page.</param>
public sealed record MoveWikiPageRequest(
    Guid? TargetParentId,
    int TargetPosition);