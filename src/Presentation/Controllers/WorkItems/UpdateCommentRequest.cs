namespace Presentation.Controllers.WorkItems;

/// <summary>
/// Request to update a comment on a work item.
/// </summary>
/// <param name="Content">The updated content of the comment.</param>
public sealed record UpdateCommentRequest(string Content);