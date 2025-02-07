namespace Contracts.Teams;

public sealed record AddMemberRequest
{
    /// <summary>
    /// The unique identifier of the member to add to the team.
    /// </summary>
    public Guid MemberId { get; init; }
}