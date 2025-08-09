namespace Presentation.Controllers.Teams;

/// <summary>
/// Request to add a user to a team.
/// </summary>
/// <param name="UserId">The unique identifier of the user to add to the team.</param>
public sealed record AddUserToTeamRequest(Guid UserId);