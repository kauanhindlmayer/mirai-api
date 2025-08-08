namespace Presentation.Controllers.WorkItems;

/// <summary>
/// Request to add a comment to a work item.
/// </summary>
/// <param name="Content">The content of the comment.</param>
public sealed record AddCommentRequest(string Content);