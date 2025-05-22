namespace WebApi.Controllers.WorkItems;

/// <summary>
/// Data transfer object for adding a comment to a work item.
/// </summary>
/// <param name="Content">The content of the comment.</param>
public sealed record AddCommentRequest(string Content);