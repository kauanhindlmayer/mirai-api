namespace Presentation.Controllers.Teams;

/// <summary>
/// Request to change a team member's role.
/// </summary>
/// <param name="RoleId">The unique identifier of the role to assign.</param>
public sealed record ChangeTeamMemberRoleRequest(Guid RoleId);
