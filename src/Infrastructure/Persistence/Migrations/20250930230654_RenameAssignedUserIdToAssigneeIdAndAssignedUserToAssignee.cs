using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameAssignedUserIdToAssigneeIdAndAssignedUserToAssignee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_work_items_users_assigned_user_id",
                table: "work_items");

            migrationBuilder.RenameColumn(
                name: "assigned_user_id",
                table: "work_items",
                newName: "assignee_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_items_assigned_user_id",
                table: "work_items",
                newName: "ix_work_items_assignee_id");

            migrationBuilder.AddForeignKey(
                name: "fk_work_items_users_assignee_id",
                table: "work_items",
                column: "assignee_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_work_items_users_assignee_id",
                table: "work_items");

            migrationBuilder.RenameColumn(
                name: "assignee_id",
                table: "work_items",
                newName: "assigned_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_items_assignee_id",
                table: "work_items",
                newName: "ix_work_items_assigned_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_work_items_users_assigned_user_id",
                table: "work_items",
                column: "assigned_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
