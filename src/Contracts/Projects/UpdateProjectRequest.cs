namespace Contracts.Projects;

public sealed record UpdateProjectRequest
{
    /// <summary>
    /// The name of the project.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The description of the project.
    /// </summary>
    public string Description { get; init; } = string.Empty;
}