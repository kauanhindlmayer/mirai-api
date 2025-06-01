namespace Presentation.Controllers.Personas;

/// <summary>
/// Data transfer object for updating a persona.
/// </summary>
/// <param name="Name">The name of the persona.</param>
/// <param name="Description">The description of the persona.</param>
/// <param name="File">The image file for the persona.</param>
public sealed record UpdatePersonaRequest(
    string Name,
    string? Description,
    IFormFile? File);