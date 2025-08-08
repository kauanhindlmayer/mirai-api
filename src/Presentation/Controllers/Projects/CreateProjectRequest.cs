namespace Presentation.Controllers.Projects;

/// <summary>
/// Request to create a new project.
/// </summary>
/// <param name="Name">The name of the project.</param>
/// <param name="Description">The description of the project.</param>
public sealed record CreateProjectRequest(
    string Name,
    string Description);