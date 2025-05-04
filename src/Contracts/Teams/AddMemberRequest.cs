namespace Contracts.Teams;

/// <summary>
/// Data transfer object for adding a member to a team.
/// </summary>
/// <param name="MemberId">The unique identifier of the member to add to the team.</param>
public sealed record AddMemberRequest(Guid MemberId);