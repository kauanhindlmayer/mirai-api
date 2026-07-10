namespace Domain.Authorization;

/// <summary>
/// The fixed catalog of system roles seeded into every environment.
/// </summary>
public static class SystemRoles
{
    public static readonly Guid OrganizationOwnerId = Guid.Parse("00000000-0000-0000-0001-000000000001");
    public static readonly Guid OrganizationAdminId = Guid.Parse("00000000-0000-0000-0001-000000000002");
    public static readonly Guid OrganizationMemberId = Guid.Parse("00000000-0000-0000-0001-000000000003");
    public static readonly Guid ProjectAdminId = Guid.Parse("00000000-0000-0000-0002-000000000001");
    public static readonly Guid ProjectContributorId = Guid.Parse("00000000-0000-0000-0002-000000000002");
    public static readonly Guid ProjectViewerId = Guid.Parse("00000000-0000-0000-0002-000000000003");
    public static readonly Guid TeamAdminId = Guid.Parse("00000000-0000-0000-0003-000000000001");
    public static readonly Guid TeamMemberId = Guid.Parse("00000000-0000-0000-0003-000000000002");

    private static readonly Permission[] AllOrganizationPermissions =
    [
        Permission.OrganizationView,
        Permission.OrganizationManage,
        Permission.OrganizationManageMembers,
        Permission.OrganizationDelete,
    ];

    private static readonly Permission[] AllProjectAndTeamPermissions =
    [
        Permission.ProjectView,
        Permission.ProjectManage,
        Permission.ProjectManageMembers,
        Permission.ProjectDelete,
        Permission.ProjectManageWorkItems,
        Permission.ProjectManageBoards,
        Permission.ProjectManageSprints,
        Permission.ProjectManageWikiPages,
        Permission.ProjectManageTags,
        Permission.ProjectManageRetrospectives,
        Permission.ProjectManagePersonas,
        Permission.ProjectViewDashboards,
        Permission.TeamView,
        Permission.TeamManage,
        Permission.TeamManageMembers,
        Permission.TeamManageSprints,
        Permission.TeamManageBoards,
        Permission.TeamManageRetrospectives,
    ];

    public static Role OrganizationOwner { get; } = Role.CreateSystemRole(
        OrganizationOwnerId,
        "Owner",
        RoleScope.Organization,
        [.. AllOrganizationPermissions, .. AllProjectAndTeamPermissions]);

    public static Role OrganizationAdmin { get; } = Role.CreateSystemRole(
        OrganizationAdminId,
        "Admin",
        RoleScope.Organization,
        [.. AllOrganizationPermissions.Where(p => p != Permission.OrganizationDelete), .. AllProjectAndTeamPermissions]);

    public static Role OrganizationMember { get; } = Role.CreateSystemRole(
        OrganizationMemberId,
        "Member",
        RoleScope.Organization,
        [Permission.OrganizationView]);

    public static Role ProjectAdmin { get; } = Role.CreateSystemRole(
        ProjectAdminId,
        "Admin",
        RoleScope.Project,
        AllProjectAndTeamPermissions);

    public static Role ProjectContributor { get; } = Role.CreateSystemRole(
        ProjectContributorId,
        "Contributor",
        RoleScope.Project,
        [
            Permission.ProjectView,
            Permission.ProjectManageWorkItems,
            Permission.ProjectManageWikiPages,
            Permission.ProjectManageTags,
            Permission.ProjectManageRetrospectives,
            Permission.ProjectViewDashboards,
            Permission.TeamView,
        ]);

    public static Role ProjectViewer { get; } = Role.CreateSystemRole(
        ProjectViewerId,
        "Viewer",
        RoleScope.Project,
        [Permission.ProjectView, Permission.ProjectViewDashboards, Permission.TeamView]);

    public static Role TeamAdmin { get; } = Role.CreateSystemRole(
        TeamAdminId,
        "Admin",
        RoleScope.Team,
        [
            Permission.TeamView,
            Permission.TeamManage,
            Permission.TeamManageMembers,
            Permission.TeamManageSprints,
            Permission.TeamManageBoards,
            Permission.TeamManageRetrospectives,
        ]);

    public static Role TeamMember { get; } = Role.CreateSystemRole(
        TeamMemberId,
        "Member",
        RoleScope.Team,
        [Permission.TeamView]);

    public static IReadOnlyCollection<Role> All { get; } =
    [
        OrganizationOwner,
        OrganizationAdmin,
        OrganizationMember,
        ProjectAdmin,
        ProjectContributor,
        ProjectViewer,
        TeamAdmin,
        TeamMember,
    ];
}
