namespace Presentation.Controllers.Teams;

/// <summary>
/// Request to create a new team.
/// </summary>
/// <param name="Name">The name of the team.</param>
/// <param name="Description">The description of the team.</param>
public sealed record CreateTeamRequest(
    string Name,
    string Description);