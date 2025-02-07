namespace Contracts.Tags;

public sealed record CreateTagRequest
{
    /// <summary>
    /// The name of the tag.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The description of the tag.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// The color of the tag in hexadecimal format.
    /// </summary>
    public required string Color { get; init; }
}