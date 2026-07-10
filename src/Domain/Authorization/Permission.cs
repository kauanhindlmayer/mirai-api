namespace Domain.Authorization;

public enum Permission
{
    OrganizationView,
    OrganizationManage,
    OrganizationManageMembers,
    OrganizationDelete,
    ProjectView,
    ProjectManage,
    ProjectManageMembers,
    ProjectDelete,
    ProjectManageWorkItems,
    ProjectManageBoards,
    ProjectManageSprints,
    ProjectManageWikiPages,
    ProjectManageTags,
    ProjectManageRetrospectives,
    ProjectManagePersonas,
    ProjectViewDashboards,
    TeamView,
    TeamManage,
    TeamManageMembers,
    TeamManageSprints,
    TeamManageBoards,
    TeamManageRetrospectives,
}
