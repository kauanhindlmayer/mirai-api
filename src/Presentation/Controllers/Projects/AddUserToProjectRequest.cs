namespace Presentation.Controllers.Projects;

/// <summary>
/// Request to add a user to a project.
/// </summary>
/// <param name="UserId">The unique identifier of the user to add.</param>
public sealed record AddUserToProjectRequest(Guid UserId);