using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "organizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organizations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projects", x => x.id);
                    table.ForeignKey(
                        name: "fk_projects_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.id);
                    table.ForeignKey(
                        name: "fk_tags_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_teams", x => x.id);
                    table.ForeignKey(
                        name: "fk_teams_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "boards",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_boards", x => x.id);
                    table.ForeignKey(
                        name: "fk_boards_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "retrospectives",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    max_votes_per_user = table.Column<int>(type: "integer", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_retrospectives", x => x.id);
                    table.ForeignKey(
                        name: "fk_retrospectives_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sprints",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sprints", x => x.id);
                    table.ForeignKey(
                        name: "fk_sprints_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    identity_id = table.Column<string>(type: "text", nullable: false),
                    image_url = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "board_columns",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    board_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    wip_limit = table.Column<int>(type: "integer", nullable: true),
                    definition_of_done = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_board_columns", x => x.id);
                    table.ForeignKey(
                        name: "fk_board_columns_boards_board_id",
                        column: x => x.board_id,
                        principalTable: "boards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "retrospective_columns",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    retrospective_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_retrospective_columns", x => x.id);
                    table.ForeignKey(
                        name: "fk_retrospective_columns_retrospectives_retrospective_id",
                        column: x => x.retrospective_id,
                        principalTable: "retrospectives",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "wiki_pages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_wiki_page_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wiki_pages", x => x.id);
                    table.ForeignKey(
                        name: "fk_wiki_pages_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_wiki_pages_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_wiki_pages_wiki_pages_parent_wiki_page_id",
                        column: x => x.parent_wiki_page_id,
                        principalTable: "wiki_pages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    acceptance_criteria = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    search_vector = table.Column<Vector>(type: "vector", nullable: true),
                    assignee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_team_id = table.Column<Guid>(type: "uuid", nullable: true),
                    parent_work_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sprint_id = table.Column<Guid>(type: "uuid", nullable: true),
                    classification_value_area = table.Column<string>(type: "text", nullable: false),
                    planning_priority = table.Column<int>(type: "integer", nullable: true),
                    planning_story_points = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_items_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_work_items_sprints_sprint_id",
                        column: x => x.sprint_id,
                        principalTable: "sprints",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_work_items_teams_assigned_team_id",
                        column: x => x.assigned_team_id,
                        principalTable: "teams",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_work_items_users_assignee_id",
                        column: x => x.assignee_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_work_items_work_items_parent_work_item_id",
                        column: x => x.parent_work_item_id,
                        principalTable: "work_items",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "retrospective_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    votes = table.Column<int>(type: "integer", nullable: false),
                    retrospective_column_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_retrospective_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_retrospective_items_retrospective_columns_retrospective_col",
                        column: x => x.retrospective_column_id,
                        principalTable: "retrospective_columns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_retrospective_items_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wiki_page_comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    wiki_page_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wiki_page_comments", x => x.id);
                    table.ForeignKey(
                        name: "fk_wiki_page_comments_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_wiki_page_comments_wiki_pages_wiki_page_id",
                        column: x => x.wiki_page_id,
                        principalTable: "wiki_pages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wiki_page_views",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    wiki_page_id = table.Column<Guid>(type: "uuid", nullable: false),
                    viewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    viewer_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wiki_page_views", x => x.id);
                    table.ForeignKey(
                        name: "fk_wiki_page_views_wiki_pages_wiki_page_id",
                        column: x => x.wiki_page_id,
                        principalTable: "wiki_pages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "board_cards",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    board_column_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_board_cards", x => x.id);
                    table.ForeignKey(
                        name: "fk_board_cards_board_columns_board_column_id",
                        column: x => x.board_column_id,
                        principalTable: "board_columns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_board_cards_work_items_work_item_id",
                        column: x => x.work_item_id,
                        principalTable: "work_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "work_item_comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_item_comments", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_item_comments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_work_item_comments_work_items_work_item_id",
                        column: x => x.work_item_id,
                        principalTable: "work_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "work_item_tags",
                columns: table => new
                {
                    tags_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_items_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_item_tags", x => new { x.tags_id, x.work_items_id });
                    table.ForeignKey(
                        name: "fk_work_item_tags_tags_tags_id",
                        column: x => x.tags_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_work_item_tags_work_items_work_items_id",
                        column: x => x.work_items_id,
                        principalTable: "work_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_board_cards_board_column_id",
                table: "board_cards",
                column: "board_column_id");

            migrationBuilder.CreateIndex(
                name: "ix_board_cards_work_item_id",
                table: "board_cards",
                column: "work_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_board_columns_board_id",
                table: "board_columns",
                column: "board_id");

            migrationBuilder.CreateIndex(
                name: "ix_boards_team_id",
                table: "boards",
                column: "team_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_organization_user_organizations_id",
                table: "organization_user",
                column: "organizations_id");

            migrationBuilder.CreateIndex(
                name: "ix_projects_organization_id",
                table: "projects",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_retrospective_columns_retrospective_id",
                table: "retrospective_columns",
                column: "retrospective_id");

            migrationBuilder.CreateIndex(
                name: "ix_retrospective_items_author_id",
                table: "retrospective_items",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_retrospective_items_retrospective_column_id",
                table: "retrospective_items",
                column: "retrospective_column_id");

            migrationBuilder.CreateIndex(
                name: "ix_retrospectives_team_id",
                table: "retrospectives",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_sprints_team_id",
                table: "sprints",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_project_id_name",
                table: "tags",
                columns: new[] { "project_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_teams_project_id",
                table: "teams",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_identity_id",
                table: "users",
                column: "identity_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_team_id",
                table: "users",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_wiki_page_comments_author_id",
                table: "wiki_page_comments",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_wiki_page_comments_wiki_page_id",
                table: "wiki_page_comments",
                column: "wiki_page_id");

            migrationBuilder.CreateIndex(
                name: "ix_wiki_page_views_wiki_page_id",
                table: "wiki_page_views",
                column: "wiki_page_id");

            migrationBuilder.CreateIndex(
                name: "ix_wiki_pages_author_id",
                table: "wiki_pages",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_wiki_pages_parent_wiki_page_id",
                table: "wiki_pages",
                column: "parent_wiki_page_id");

            migrationBuilder.CreateIndex(
                name: "ix_wiki_pages_project_id_parent_wiki_page_id_position",
                table: "wiki_pages",
                columns: new[] { "project_id", "parent_wiki_page_id", "position" });

            migrationBuilder.CreateIndex(
                name: "ix_work_item_comments_user_id",
                table: "work_item_comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_item_comments_work_item_id",
                table: "work_item_comments",
                column: "work_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_item_tags_work_items_id",
                table: "work_item_tags",
                column: "work_items_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_items_assigned_team_id",
                table: "work_items",
                column: "assigned_team_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_items_assignee_id",
                table: "work_items",
                column: "assignee_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_items_parent_work_item_id",
                table: "work_items",
                column: "parent_work_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_items_project_id_code",
                table: "work_items",
                columns: new[] { "project_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_work_items_sprint_id",
                table: "work_items",
                column: "sprint_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "board_cards");

            migrationBuilder.DropTable(
                name: "organization_user");

            migrationBuilder.DropTable(
                name: "retrospective_items");

            migrationBuilder.DropTable(
                name: "wiki_page_comments");

            migrationBuilder.DropTable(
                name: "wiki_page_views");

            migrationBuilder.DropTable(
                name: "work_item_comments");

            migrationBuilder.DropTable(
                name: "work_item_tags");

            migrationBuilder.DropTable(
                name: "board_columns");

            migrationBuilder.DropTable(
                name: "retrospective_columns");

            migrationBuilder.DropTable(
                name: "wiki_pages");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "work_items");

            migrationBuilder.DropTable(
                name: "boards");

            migrationBuilder.DropTable(
                name: "retrospectives");

            migrationBuilder.DropTable(
                name: "sprints");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "organizations");
        }
    }
}
