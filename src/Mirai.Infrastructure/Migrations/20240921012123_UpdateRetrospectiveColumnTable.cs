using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mirai.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRetrospectiveColumnTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RetrospectiveColumn_Retrospectives_RetrospectiveId",
                table: "RetrospectiveColumn");

            migrationBuilder.DropForeignKey(
                name: "FK_RetrospectiveItem_RetrospectiveColumn_RetrospectiveColumnId",
                table: "RetrospectiveItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RetrospectiveItem",
                table: "RetrospectiveItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RetrospectiveColumn",
                table: "RetrospectiveColumn");

            migrationBuilder.RenameTable(
                name: "RetrospectiveItem",
                newName: "RetrospectiveItems");

            migrationBuilder.RenameTable(
                name: "RetrospectiveColumn",
                newName: "RetrospectiveColumns");

            migrationBuilder.RenameIndex(
                name: "IX_RetrospectiveItem_RetrospectiveColumnId",
                table: "RetrospectiveItems",
                newName: "IX_RetrospectiveItems_RetrospectiveColumnId");

            migrationBuilder.RenameIndex(
                name: "IX_RetrospectiveColumn_RetrospectiveId",
                table: "RetrospectiveColumns",
                newName: "IX_RetrospectiveColumns_RetrospectiveId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RetrospectiveItems",
                table: "RetrospectiveItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RetrospectiveColumns",
                table: "RetrospectiveColumns",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RetrospectiveColumns_Retrospectives_RetrospectiveId",
                table: "RetrospectiveColumns",
                column: "RetrospectiveId",
                principalTable: "Retrospectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RetrospectiveItems_RetrospectiveColumns_RetrospectiveColumnId",
                table: "RetrospectiveItems",
                column: "RetrospectiveColumnId",
                principalTable: "RetrospectiveColumns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RetrospectiveColumns_Retrospectives_RetrospectiveId",
                table: "RetrospectiveColumns");

            migrationBuilder.DropForeignKey(
                name: "FK_RetrospectiveItems_RetrospectiveColumns_RetrospectiveColumnId",
                table: "RetrospectiveItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RetrospectiveItems",
                table: "RetrospectiveItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RetrospectiveColumns",
                table: "RetrospectiveColumns");

            migrationBuilder.RenameTable(
                name: "RetrospectiveItems",
                newName: "RetrospectiveItem");

            migrationBuilder.RenameTable(
                name: "RetrospectiveColumns",
                newName: "RetrospectiveColumn");

            migrationBuilder.RenameIndex(
                name: "IX_RetrospectiveItems_RetrospectiveColumnId",
                table: "RetrospectiveItem",
                newName: "IX_RetrospectiveItem_RetrospectiveColumnId");

            migrationBuilder.RenameIndex(
                name: "IX_RetrospectiveColumns_RetrospectiveId",
                table: "RetrospectiveColumn",
                newName: "IX_RetrospectiveColumn_RetrospectiveId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RetrospectiveItem",
                table: "RetrospectiveItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RetrospectiveColumn",
                table: "RetrospectiveColumn",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RetrospectiveColumn_Retrospectives_RetrospectiveId",
                table: "RetrospectiveColumn",
                column: "RetrospectiveId",
                principalTable: "Retrospectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RetrospectiveItem_RetrospectiveColumn_RetrospectiveColumnId",
                table: "RetrospectiveItem",
                column: "RetrospectiveColumnId",
                principalTable: "RetrospectiveColumn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
