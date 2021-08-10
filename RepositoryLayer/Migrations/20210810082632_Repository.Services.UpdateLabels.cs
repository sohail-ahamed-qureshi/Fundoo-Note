using Microsoft.EntityFrameworkCore.Migrations;

namespace RepositoryLayer.Migrations
{
    public partial class RepositoryServicesUpdateLabels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_labelTable_DbNotes_NoteId",
                table: "labelTable");

            migrationBuilder.RenameColumn(
                name: "NoteId",
                table: "labelTable",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_labelTable_NoteId",
                table: "labelTable",
                newName: "IX_labelTable_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_labelTable_FundooUsers_UserId",
                table: "labelTable",
                column: "UserId",
                principalTable: "FundooUsers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_labelTable_FundooUsers_UserId",
                table: "labelTable");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "labelTable",
                newName: "NoteId");

            migrationBuilder.RenameIndex(
                name: "IX_labelTable_UserId",
                table: "labelTable",
                newName: "IX_labelTable_NoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_labelTable_DbNotes_NoteId",
                table: "labelTable",
                column: "NoteId",
                principalTable: "DbNotes",
                principalColumn: "NoteId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
