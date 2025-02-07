namespace Contracts.Projects;

public sealed record CreateProjectRequest
{
    /// <summary>
    /// The name of the project.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The description of the project.
    /// </summary>
    public required string Description { get; init; }
}