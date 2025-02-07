namespace Contracts.WikiPages;

public sealed record MoveWikiPageRequest
{
    /// <summary>
    /// The unique identifier of the target parent page.
    /// </summary>
    public Guid? TargetParentId { get; init; }

    /// <summary>
    /// The target position of the page.
    /// </summary>
    public int TargetPosition { get; init; }
}