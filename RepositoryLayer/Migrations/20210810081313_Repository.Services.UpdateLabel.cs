using Microsoft.EntityFrameworkCore.Migrations;

namespace RepositoryLayer.Migrations
{
    public partial class RepositoryServicesUpdateLabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NoteId",
                table: "labelTable",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_labelTable_NoteId",
                table: "labelTable",
                column: "NoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_labelTable_DbNotes_NoteId",
                table: "labelTable",
                column: "NoteId",
                principalTable: "DbNotes",
                principalColumn: "NoteId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_labelTable_DbNotes_NoteId",
                table: "labelTable");

            migrationBuilder.DropIndex(
                name: "IX_labelTable_NoteId",
                table: "labelTable");

            migrationBuilder.DropColumn(
                name: "NoteId",
                table: "labelTable");
        }
    }
}
