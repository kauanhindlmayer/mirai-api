namespace Presentation.Controllers.Organizations;

/// <summary>
/// Request to change an organization member's role.
/// </summary>
/// <param name="RoleId">The unique identifier of the role to assign.</param>
public sealed record ChangeOrganizationMemberRoleRequest(Guid RoleId);
