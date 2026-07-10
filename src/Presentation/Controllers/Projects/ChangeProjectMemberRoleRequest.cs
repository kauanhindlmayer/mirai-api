namespace Presentation.Controllers.Projects;

/// <summary>
/// Request to change a project member's role.
/// </summary>
/// <param name="RoleId">The unique identifier of the role to assign.</param>
public sealed record ChangeProjectMemberRoleRequest(Guid RoleId);
