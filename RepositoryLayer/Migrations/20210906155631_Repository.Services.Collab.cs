using Microsoft.EntityFrameworkCore.Migrations;

namespace RepositoryLayer.Migrations
{
    public partial class RepositoryServicesCollab : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JunctionUserCollabs",
                columns: table => new
                {
                    CollabId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotesNoteId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JunctionUserCollabs", x => x.CollabId);
                    table.ForeignKey(
                        name: "FK_JunctionUserCollabs_DbNotes_NotesNoteId",
                        column: x => x.NotesNoteId,
                        principalTable: "DbNotes",
                        principalColumn: "NoteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JunctionUserCollabs_FundooUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "FundooUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JunctionUserCollabs_NotesNoteId",
                table: "JunctionUserCollabs",
                column: "NotesNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_JunctionUserCollabs_UserId",
                table: "JunctionUserCollabs",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JunctionUserCollabs");
        }
    }
}
