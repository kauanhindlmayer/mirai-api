namespace Presentation.Controllers.Tags;

/// <summary>
/// Data transfer object for merging tags.
/// </summary>
/// <param name="TargetTagId">
/// The unique identifier of the tag that will remain after the merge.
/// </param>
/// <param name="SourceTagIds">
/// A list of tag IDs to be merged into the target tag and then removed.
/// </param>
public sealed record MergeTagsRequest(
    Guid TargetTagId,
    List<Guid> SourceTagIds);