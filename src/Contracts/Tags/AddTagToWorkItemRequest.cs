namespace Contracts.Tags;

public sealed record AddTagToWorkItemRequest
{
    /// <summary>
    /// The name of the tag.
    /// </summary>
    public required string Name { get; init; }
}