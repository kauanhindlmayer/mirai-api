using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersToTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_teams_team_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "fk_wiki_page_comments_users_author_id",
                table: "wiki_page_comments");

            migrationBuilder.DropForeignKey(
                name: "fk_work_items_teams_assigned_team_id",
                table: "work_items");

            migrationBuilder.DropForeignKey(
                name: "fk_work_items_users_assignee_id",
                table: "work_items");

            migrationBuilder.DropIndex(
                name: "ix_users_team_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_board_cards_work_item_id",
                table: "board_cards");

            migrationBuilder.DropColumn(
                name: "team_id",
                table: "users");

            migrationBuilder.RenameTable(
                name: "project_users",
                newName: "ProjectUsers");

            migrationBuilder.RenameTable(
                name: "organization_users",
                newName: "OrganizationUsers");

            migrationBuilder.RenameColumn(
                name: "assignee_id",
                table: "work_items",
                newName: "assigned_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_items_assignee_id",
                table: "work_items",
                newName: "ix_work_items_assigned_user_id");

            migrationBuilder.AlterColumn<string>(
                name: "acceptance_criteria",
                table: "work_items",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "TeamUsers",
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
                name: "ix_work_items_status",
                table: "work_items",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_work_items_type",
                table: "work_items",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "ix_wiki_page_views_viewer_id",
                table: "wiki_page_views",
                column: "viewer_id");

            migrationBuilder.CreateIndex(
                name: "ix_teams_project_id_name",
                table: "teams",
                columns: new[] { "project_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_board_columns_board_id_position",
                table: "board_columns",
                columns: new[] { "board_id", "position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_board_cards_board_column_id_position",
                table: "board_cards",
                columns: new[] { "board_column_id", "position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_board_cards_work_item_id",
                table: "board_cards",
                column: "work_item_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_project_users_project_id",
                table: "ProjectUsers",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_users_organization_id",
                table: "OrganizationUsers",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_users_team_id",
                table: "TeamUsers",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_users_user_id",
                table: "TeamUsers",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_wiki_page_comments_users_author_id",
                table: "wiki_page_comments",
                column: "author_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_wiki_page_views_users_viewer_id",
                table: "wiki_page_views",
                column: "viewer_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_items_teams_assigned_team_id",
                table: "work_items",
                column: "assigned_team_id",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_work_items_users_assigned_user_id",
                table: "work_items",
                column: "assigned_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_wiki_page_comments_users_author_id",
                table: "wiki_page_comments");

            migrationBuilder.DropForeignKey(
                name: "fk_wiki_page_views_users_viewer_id",
                table: "wiki_page_views");

            migrationBuilder.DropForeignKey(
                name: "fk_work_items_teams_assigned_team_id",
                table: "work_items");

            migrationBuilder.DropForeignKey(
                name: "fk_work_items_users_assigned_user_id",
                table: "work_items");

            migrationBuilder.DropTable(
                name: "TeamUsers");

            migrationBuilder.DropIndex(
                name: "ix_work_items_status",
                table: "work_items");

            migrationBuilder.DropIndex(
                name: "ix_work_items_type",
                table: "work_items");

            migrationBuilder.DropIndex(
                name: "ix_wiki_page_views_viewer_id",
                table: "wiki_page_views");

            migrationBuilder.DropIndex(
                name: "ix_teams_project_id_name",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "ix_board_columns_board_id_position",
                table: "board_columns");

            migrationBuilder.DropIndex(
                name: "ix_board_cards_board_column_id_position",
                table: "board_cards");

            migrationBuilder.DropIndex(
                name: "ix_board_cards_work_item_id",
                table: "board_cards");

            migrationBuilder.DropIndex(
                name: "ix_project_users_project_id",
                table: "ProjectUsers");

            migrationBuilder.DropIndex(
                name: "ix_organization_users_organization_id",
                table: "OrganizationUsers");

            migrationBuilder.RenameTable(
                name: "ProjectUsers",
                newName: "project_users");

            migrationBuilder.RenameTable(
                name: "OrganizationUsers",
                newName: "organization_users");

            migrationBuilder.RenameColumn(
                name: "assigned_user_id",
                table: "work_items",
                newName: "assignee_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_items_assigned_user_id",
                table: "work_items",
                newName: "ix_work_items_assignee_id");

            migrationBuilder.AlterColumn<string>(
                name: "acceptance_criteria",
                table: "work_items",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "team_id",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_team_id",
                table: "users",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_board_cards_work_item_id",
                table: "board_cards",
                column: "work_item_id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_teams_team_id",
                table: "users",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_wiki_page_comments_users_author_id",
                table: "wiki_page_comments",
                column: "author_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_work_items_teams_assigned_team_id",
                table: "work_items",
                column: "assigned_team_id",
                principalTable: "teams",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_work_items_users_assignee_id",
                table: "work_items",
                column: "assignee_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
