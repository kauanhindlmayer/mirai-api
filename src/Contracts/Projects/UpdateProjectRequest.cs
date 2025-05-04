namespace Contracts.Projects;

/// <summary>
/// Data transfer object for updating a project.
/// </summary>
/// <param name="Name">The name of the project.</param>
/// <param name="Description">The description of the project.</param>
public sealed record UpdateProjectRequest(
    string Name,
    string Description);