namespace Contracts.WikiPages;

public record MoveWikiPageRequest(Guid? TargetParentId, int TargetPosition);