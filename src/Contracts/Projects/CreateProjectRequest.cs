namespace Contracts.Projects;

/// <summary>
/// Data transfer object for creating a project.
/// </summary>
/// <param name="Name">The name of the project.</param>
/// <param name="Description">The description of the project.</param>
public sealed record CreateProjectRequest(
    string Name,
    string Description);