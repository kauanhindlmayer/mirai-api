using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleBasedAccessControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "organization_users");

            migrationBuilder.DropTable(
                name: "project_users");

            migrationBuilder.DropTable(
                name: "team_users");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    scope = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_system_role = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "organization_members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_organization_members_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_organization_members_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_organization_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_members_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_project_members_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_project_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_permissions", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "team_members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_team_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_team_members_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_team_members_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_team_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "created_at_utc", "is_system_role", "name", "scope", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Owner", "Organization", null },
                    { new Guid("00000000-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Admin", "Organization", null },
                    { new Guid("00000000-0000-0000-0001-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Member", "Organization", null },
                    { new Guid("00000000-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Admin", "Project", null },
                    { new Guid("00000000-0000-0000-0002-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Contributor", "Project", null },
                    { new Guid("00000000-0000-0000-0002-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Viewer", "Project", null },
                    { new Guid("00000000-0000-0000-0003-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Admin", "Team", null },
                    { new Guid("00000000-0000-0000-0003-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Member", "Team", null }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "id", "created_at_utc", "permission", "role_id", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OrganizationView", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("00000000-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OrganizationView", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("00000000-0000-0000-0001-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OrganizationView", new Guid("00000000-0000-0000-0001-000000000003"), null },
                    { new Guid("00000001-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OrganizationManage", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("00000001-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OrganizationManage", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("00000002-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OrganizationManageMembers", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("00000002-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OrganizationManageMembers", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("00000003-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OrganizationDelete", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("00000004-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectView", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("00000004-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectView", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("00000004-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectView", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("00000004-0000-0000-0002-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectView", new Guid("00000000-0000-0000-0002-000000000002"), null },
                    { new Guid("00000004-0000-0000-0002-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectView", new Guid("00000000-0000-0000-0002-000000000003"), null },
                    { new Guid("00000005-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManage", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("00000005-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManage", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("00000005-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManage", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("00000006-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageMembers", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("00000006-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageMembers", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("00000006-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageMembers", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("00000007-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectDelete", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("00000007-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectDelete", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("00000007-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectDelete", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("00000008-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageWorkItems", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("00000008-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageWorkItems", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("00000008-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageWorkItems", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("00000008-0000-0000-0002-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageWorkItems", new Guid("00000000-0000-0000-0002-000000000002"), null },
                    { new Guid("00000009-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageBoards", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("00000009-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageBoards", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("00000009-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageBoards", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("0000000a-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageSprints", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("0000000a-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageSprints", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("0000000a-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageSprints", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("0000000b-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageWikiPages", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("0000000b-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageWikiPages", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("0000000b-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageWikiPages", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("0000000b-0000-0000-0002-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageWikiPages", new Guid("00000000-0000-0000-0002-000000000002"), null },
                    { new Guid("0000000c-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageTags", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("0000000c-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageTags", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("0000000c-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageTags", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("0000000c-0000-0000-0002-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageTags", new Guid("00000000-0000-0000-0002-000000000002"), null },
                    { new Guid("0000000d-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageRetrospectives", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("0000000d-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageRetrospectives", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("0000000d-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageRetrospectives", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("0000000d-0000-0000-0002-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManageRetrospectives", new Guid("00000000-0000-0000-0002-000000000002"), null },
                    { new Guid("0000000e-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManagePersonas", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("0000000e-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManagePersonas", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("0000000e-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectManagePersonas", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("0000000f-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectViewDashboards", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("0000000f-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectViewDashboards", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("0000000f-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectViewDashboards", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("0000000f-0000-0000-0002-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectViewDashboards", new Guid("00000000-0000-0000-0002-000000000002"), null },
                    { new Guid("0000000f-0000-0000-0002-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ProjectViewDashboards", new Guid("00000000-0000-0000-0002-000000000003"), null },
                    { new Guid("00000010-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamView", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("00000010-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamView", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("00000010-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamView", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("00000010-0000-0000-0002-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamView", new Guid("00000000-0000-0000-0002-000000000002"), null },
                    { new Guid("00000010-0000-0000-0002-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamView", new Guid("00000000-0000-0000-0002-000000000003"), null },
                    { new Guid("00000010-0000-0000-0003-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamView", new Guid("00000000-0000-0000-0003-000000000001"), null },
                    { new Guid("00000010-0000-0000-0003-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamView", new Guid("00000000-0000-0000-0003-000000000002"), null },
                    { new Guid("00000011-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManage", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("00000011-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManage", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("00000011-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManage", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("00000011-0000-0000-0003-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManage", new Guid("00000000-0000-0000-0003-000000000001"), null },
                    { new Guid("00000012-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManageMembers", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("00000012-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManageMembers", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("00000012-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManageMembers", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("00000012-0000-0000-0003-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManageMembers", new Guid("00000000-0000-0000-0003-000000000001"), null },
                    { new Guid("00000013-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManageSprints", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("00000013-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManageSprints", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("00000013-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManageSprints", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("00000013-0000-0000-0003-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManageSprints", new Guid("00000000-0000-0000-0003-000000000001"), null },
                    { new Guid("00000014-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManageBoards", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("00000014-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManageBoards", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("00000014-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManageBoards", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("00000014-0000-0000-0003-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManageBoards", new Guid("00000000-0000-0000-0003-000000000001"), null },
                    { new Guid("00000015-0000-0000-0001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManageRetrospectives", new Guid("00000000-0000-0000-0001-000000000001"), null },
                    { new Guid("00000015-0000-0000-0001-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManageRetrospectives", new Guid("00000000-0000-0000-0001-000000000002"), null },
                    { new Guid("00000015-0000-0000-0002-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManageRetrospectives", new Guid("00000000-0000-0000-0002-000000000001"), null },
                    { new Guid("00000015-0000-0000-0003-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TeamManageRetrospectives", new Guid("00000000-0000-0000-0003-000000000001"), null }
                });

            migrationBuilder.CreateIndex(
                name: "ix_organization_members_organization_id_user_id",
                table: "organization_members",
                columns: new[] { "organization_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_organization_members_role_id",
                table: "organization_members",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_members_user_id",
                table: "organization_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_members_project_id_user_id",
                table: "project_members",
                columns: new[] { "project_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_project_members_role_id",
                table: "project_members",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_members_user_id",
                table: "project_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_role_id_permission",
                table: "role_permissions",
                columns: new[] { "role_id", "permission" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_team_members_role_id",
                table: "team_members",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_members_team_id_user_id",
                table: "team_members",
                columns: new[] { "team_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_team_members_user_id",
                table: "team_members",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "organization_members");

            migrationBuilder.DropTable(
                name: "project_members");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "team_members");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.CreateTable(
                name: "organization_users",
                columns: table => new
                {
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_users", x => new { x.organization_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_organization_users_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_organization_users_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_users",
                columns: table => new
                {
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_users", x => new { x.project_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_project_users_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_project_users_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "team_users",
                columns: table => new
                {
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_team_users", x => new { x.team_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_team_users_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_team_users_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_organization_users_organization_id",
                table: "organization_users",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_users_user_id",
                table: "organization_users",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_users_project_id",
                table: "project_users",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_users_user_id",
                table: "project_users",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_users_team_id",
                table: "team_users",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_users_user_id",
                table: "team_users",
                column: "user_id");
        }
    }
}
