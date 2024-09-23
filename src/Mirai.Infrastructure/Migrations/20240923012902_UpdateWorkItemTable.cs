using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mirai.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWorkItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Classification_ValueArea",
                table: "WorkItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Code",
                table: "WorkItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Planning_Priority",
                table: "WorkItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Planning_StoryPoints",
                table: "WorkItems",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Classification_ValueArea",
                table: "WorkItems");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "WorkItems");

            migrationBuilder.DropColumn(
                name: "Planning_Priority",
                table: "WorkItems");

            migrationBuilder.DropColumn(
                name: "Planning_StoryPoints",
                table: "WorkItems");
        }
    }
}
