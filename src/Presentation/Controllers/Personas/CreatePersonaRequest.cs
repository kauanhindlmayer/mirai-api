namespace Presentation.Controllers.Personas;

/// <summary>
/// Request to create a new persona.
/// </summary>
/// <param name="Name">The name of the persona.</param>
/// <param name="Description">The description of the persona.</param>
/// <param name="File">The image file for the persona.</param>
public sealed record CreatePersonaRequest(
    string Name,
    string? Category,
    string? Description,
    IFormFile? File);