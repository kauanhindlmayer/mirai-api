namespace Presentation.Controllers.Tags;

/// <summary>
/// Request to delete tags.
/// </summary>
/// <param name="TagIds">The unique identifiers of the tags to delete.</param>
public sealed record DeleteTagsRequest(List<Guid> TagIds);