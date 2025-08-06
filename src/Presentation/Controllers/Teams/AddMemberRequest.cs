namespace Presentation.Controllers.Teams;

/// <summary>
/// Request to add a member to a team.
/// </summary>
/// <param name="MemberId">The unique identifier of the member to add to the team.</param>
public sealed record AddMemberRequest(Guid MemberId);