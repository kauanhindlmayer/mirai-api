namespace Presentation.Controllers.Tags;

/// <summary>
/// Data transfer object for deleting multiple tags.
/// </summary>
/// <param name="TagIds">The unique identifiers of the tags to delete.</param>
public sealed record DeleteTagsRequest(List<Guid> TagIds);