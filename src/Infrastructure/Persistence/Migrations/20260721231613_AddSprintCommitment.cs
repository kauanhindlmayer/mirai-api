using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSprintCommitment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sprint_commitments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sprint_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    committed_story_points = table.Column<int>(type: "integer", nullable: true),
                    status_at_start = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sprint_commitments", x => x.id);
                    table.ForeignKey(
                        name: "fk_sprint_commitments_sprints_sprint_id",
                        column: x => x.sprint_id,
                        principalTable: "sprints",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_sprint_commitments_sprint_id_work_item_id",
                table: "sprint_commitments",
                columns: new[] { "sprint_id", "work_item_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sprint_commitments");
        }
    }
}
