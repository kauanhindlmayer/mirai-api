using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkItemChangeHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "work_item_change_sets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    changed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    system_actor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_item_change_sets", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_item_change_sets_users_changed_by_user_id",
                        column: x => x.changed_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_work_item_change_sets_work_items_work_item_id",
                        column: x => x.work_item_id,
                        principalTable: "work_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "work_item_changes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_item_change_set_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    old_value = table.Column<string>(type: "text", nullable: true),
                    new_value = table.Column<string>(type: "text", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_item_changes", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_item_changes_work_item_change_sets_work_item_change_se",
                        column: x => x.work_item_change_set_id,
                        principalTable: "work_item_change_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_work_item_change_sets_changed_by_user_id",
                table: "work_item_change_sets",
                column: "changed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_item_change_sets_work_item_id",
                table: "work_item_change_sets",
                column: "work_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_item_changes_work_item_change_set_id",
                table: "work_item_changes",
                column: "work_item_change_set_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "work_item_changes");

            migrationBuilder.DropTable(
                name: "work_item_change_sets");
        }
    }
}
