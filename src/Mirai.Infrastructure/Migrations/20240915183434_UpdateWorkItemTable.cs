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
            migrationBuilder.DropForeignKey(
                name: "FK_WikiPages_WikiPages_ParentWikiPageId",
                table: "WikiPages");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "WorkItems",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "WorkItems",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_WikiPages_WikiPages_ParentWikiPageId",
                table: "WikiPages",
                column: "ParentWikiPageId",
                principalTable: "WikiPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WikiPages_WikiPages_ParentWikiPageId",
                table: "WikiPages");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "WorkItems",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "WorkItems",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_WikiPages_WikiPages_ParentWikiPageId",
                table: "WikiPages",
                column: "ParentWikiPageId",
                principalTable: "WikiPages",
                principalColumn: "Id");
        }
    }
}
