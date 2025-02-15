namespace Contracts.Retrospectives;

public sealed record CreateItemRequest
{
    /// <summary>
    /// The content of the item.
    /// </summary>
    public string Content { get; init; } = string.Empty;
}