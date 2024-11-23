namespace Contracts.WikiPages;

public sealed record MoveWikiPageRequest(
    Guid? TargetParentId,
    int TargetPosition);