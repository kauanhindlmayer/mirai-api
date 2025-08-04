using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersToOrganizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_work_items_users_assignee_id",
                table: "work_items");

            migrationBuilder.DropTable(
                name: "organization_user");

            migrationBuilder.AddColumn<Guid>(
                name: "project_id",
                table: "users",
                type: "uuid",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "ix_users_project_id",
                table: "users",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_users_user_id",
                table: "organization_users",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_projects_project_id",
                table: "users",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_work_items_users_assignee_id",
                table: "work_items",
                column: "assignee_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_projects_project_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "fk_work_items_users_assignee_id",
                table: "work_items");

            migrationBuilder.DropTable(
                name: "organization_users");

            migrationBuilder.DropIndex(
                name: "ix_users_project_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "project_id",
                table: "users");

            migrationBuilder.CreateTable(
                name: "organization_user",
                columns: table => new
                {
                    members_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organizations_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_user", x => new { x.members_id, x.organizations_id });
                    table.ForeignKey(
                        name: "fk_organization_user_organizations_organizations_id",
                        column: x => x.organizations_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_organization_user_users_members_id",
                        column: x => x.members_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_organization_user_organizations_id",
                table: "organization_user",
                column: "organizations_id");

            migrationBuilder.AddForeignKey(
                name: "fk_work_items_users_assignee_id",
                table: "work_items",
                column: "assignee_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
