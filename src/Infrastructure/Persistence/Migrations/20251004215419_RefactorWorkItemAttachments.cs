using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorWorkItemAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_work_item_attachment_blob_name",
                table: "work_item_attachment");

            migrationBuilder.DropColumn(
                name: "blob_name",
                table: "work_item_attachment");

            migrationBuilder.RenameColumn(
                name: "uploaded_by_id",
                table: "work_item_attachment",
                newName: "blob_id");

            migrationBuilder.AddColumn<Guid>(
                name: "author_id",
                table: "work_item_attachment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_work_item_attachment_blob_id",
                table: "work_item_attachment",
                column: "blob_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_work_item_attachment_blob_id",
                table: "work_item_attachment");

            migrationBuilder.DropColumn(
                name: "author_id",
                table: "work_item_attachment");

            migrationBuilder.RenameColumn(
                name: "blob_id",
                table: "work_item_attachment",
                newName: "uploaded_by_id");

            migrationBuilder.AddColumn<string>(
                name: "blob_name",
                table: "work_item_attachment",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_work_item_attachment_blob_name",
                table: "work_item_attachment",
                column: "blob_name");
        }
    }
}
