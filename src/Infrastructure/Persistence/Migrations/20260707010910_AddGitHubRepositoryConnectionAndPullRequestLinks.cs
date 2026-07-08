using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGitHubRepositoryConnectionAndPullRequestLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "git_hub_repository_connection",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    installation_id = table.Column<long>(type: "bigint", nullable: false),
                    repository_id = table.Column<long>(type: "bigint", nullable: false),
                    repository_owner = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    repository_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    connected_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    connected_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_synced_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_git_hub_repository_connection", x => x.id);
                    table.ForeignKey(
                        name: "fk_git_hub_repository_connection_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "work_item_pull_request_link",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pull_request_id = table.Column<long>(type: "bigint", nullable: false),
                    pull_request_number = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    html_url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    author_login = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    source = table.Column<string>(type: "text", nullable: false),
                    linked_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_item_pull_request_link", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_item_pull_request_link_work_items_work_item_id",
                        column: x => x.work_item_id,
                        principalTable: "work_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_git_hub_repository_connection_project_id",
                table: "git_hub_repository_connection",
                column: "project_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_git_hub_repository_connection_repository_id",
                table: "git_hub_repository_connection",
                column: "repository_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_work_item_pull_request_link_pull_request_id",
                table: "work_item_pull_request_link",
                column: "pull_request_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_item_pull_request_link_work_item_id_pull_request_id",
                table: "work_item_pull_request_link",
                columns: new[] { "work_item_id", "pull_request_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "git_hub_repository_connection");

            migrationBuilder.DropTable(
                name: "work_item_pull_request_link");
        }
    }
}
