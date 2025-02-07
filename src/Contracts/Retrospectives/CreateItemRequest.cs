namespace Contracts.Retrospectives;

public sealed record CreateItemRequest
{
    /// <summary>
    /// The description of the item.
    /// </summary>
    public string Description { get; init; } = string.Empty;
}