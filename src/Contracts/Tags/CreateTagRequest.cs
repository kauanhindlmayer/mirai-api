namespace Contracts.Tags;

/// <summary>
/// Data transfer object for creating a tag.
/// </summary>
/// <param name="Name">The name of the tag.</param>
/// <param name="Description">The description of the tag.</param>
/// <param name="Color">The color of the tag in hexadecimal format.</param>
public sealed record CreateTagRequest(
    string Name,
    string Description,
    string Color);