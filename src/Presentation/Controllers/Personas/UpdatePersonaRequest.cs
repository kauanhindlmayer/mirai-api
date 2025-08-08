namespace Presentation.Controllers.Personas;

/// <summary>
/// Request to update an existing persona.
/// </summary>
/// <param name="Name">The name of the persona.</param>
/// <param name="Description">The description of the persona.</param>
/// <param name="File">The image file for the persona.</param>
public sealed record UpdatePersonaRequest(
    string Name,
    string? Description,
    IFormFile? File);