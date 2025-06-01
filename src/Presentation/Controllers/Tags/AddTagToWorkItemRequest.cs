namespace Presentation.Controllers.Tags;

/// <summary>
/// Data transfer object for adding a tag to a work item.
/// </summary>
/// <param name="Name">The name of the tag.</param>
public sealed record AddTagToWorkItemRequest(string Name);