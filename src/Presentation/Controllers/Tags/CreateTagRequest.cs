namespace Presentation.Controllers.Tags;

/// <summary>
/// Request to create a new tag.
/// </summary>
/// <param name="Name">The name of the tag.</param>
/// <param name="Description">The description of the tag.</param>
/// <param name="Color">The color of the tag in hexadecimal format.</param>
public sealed record CreateTagRequest(
    string Name,
    string Description,
    string Color);