using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mirai.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBoardColumnTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "WorkItems",
                type: "NVARCHAR(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AcceptanceCriteria",
                table: "WorkItems",
                type: "NVARCHAR(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "WorkItemComments",
                type: "NVARCHAR(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "WikiPages",
                type: "NVARCHAR(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "WikiPageComment",
                type: "NVARCHAR(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "WipLimit",
                table: "BoardColumns",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "WorkItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(MAX)");

            migrationBuilder.AlterColumn<string>(
                name: "AcceptanceCriteria",
                table: "WorkItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(MAX)");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "WorkItemComments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(MAX)");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "WikiPages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(MAX)");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "WikiPageComment",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(MAX)");

            migrationBuilder.AlterColumn<int>(
                name: "WipLimit",
                table: "BoardColumns",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
