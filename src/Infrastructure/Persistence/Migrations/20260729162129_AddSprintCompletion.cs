using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSprintCompletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "completed_at_utc",
                table: "sprints",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "completed_story_points",
                table: "sprints",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "completed_work_item_count",
                table: "sprints",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "completed_at_utc",
                table: "sprints");

            migrationBuilder.DropColumn(
                name: "completed_story_points",
                table: "sprints");

            migrationBuilder.DropColumn(
                name: "completed_work_item_count",
                table: "sprints");
        }
    }
}
