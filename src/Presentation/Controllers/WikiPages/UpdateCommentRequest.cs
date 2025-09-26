namespace Presentation.Controllers.WikiPages;

/// <summary>
/// Request to update a comment on a wiki page.
/// </summary>
/// <param name="Content">The updated content of the comment.</param>
public sealed record UpdateCommentRequest(string Content);