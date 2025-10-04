using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkItemLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "work_item_link",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_work_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_work_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    link_type = table.Column<string>(type: "text", nullable: false),
                    comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_item_link", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_item_link_work_items_source_work_item_id",
                        column: x => x.source_work_item_id,
                        principalTable: "work_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_work_item_link_work_items_target_work_item_id",
                        column: x => x.target_work_item_id,
                        principalTable: "work_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_work_item_link_link_type",
                table: "work_item_link",
                column: "link_type");

            migrationBuilder.CreateIndex(
                name: "ix_work_item_link_source_work_item_id",
                table: "work_item_link",
                column: "source_work_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_item_link_source_work_item_id_target_work_item_id_link",
                table: "work_item_link",
                columns: new[] { "source_work_item_id", "target_work_item_id", "link_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_work_item_link_target_work_item_id",
                table: "work_item_link",
                column: "target_work_item_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "work_item_link");
        }
    }
}
