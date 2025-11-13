using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovePositionUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_board_columns_board_id_position",
                table: "board_columns");

            migrationBuilder.DropIndex(
                name: "ix_board_cards_board_column_id_position",
                table: "board_cards");

            migrationBuilder.CreateIndex(
                name: "ix_board_columns_board_id_position",
                table: "board_columns",
                columns: new[] { "board_id", "position" });

            migrationBuilder.CreateIndex(
                name: "ix_board_cards_board_column_id_position",
                table: "board_cards",
                columns: new[] { "board_column_id", "position" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_board_columns_board_id_position",
                table: "board_columns");

            migrationBuilder.DropIndex(
                name: "ix_board_cards_board_column_id_position",
                table: "board_cards");

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
        }
    }
}
