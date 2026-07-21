using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSprintLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "started_at_utc",
                table: "sprints",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "sprints",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Planned");

            migrationBuilder.CreateIndex(
                name: "ix_sprints_team_id_active",
                table: "sprints",
                column: "team_id",
                unique: true,
                filter: "\"status\" = 'Active'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sprints_team_id_active",
                table: "sprints");

            migrationBuilder.DropColumn(
                name: "started_at_utc",
                table: "sprints");

            migrationBuilder.DropColumn(
                name: "status",
                table: "sprints");
        }
    }
}
