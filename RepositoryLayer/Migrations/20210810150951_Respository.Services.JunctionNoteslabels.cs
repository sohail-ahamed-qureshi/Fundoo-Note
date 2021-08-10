using Microsoft.EntityFrameworkCore.Migrations;

namespace RepositoryLayer.Migrations
{
    public partial class RespositoryServicesJunctionNoteslabels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JunctionNotesLabels",
                columns: table => new
                {
                    JunctionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotesNoteId = table.Column<int>(type: "int", nullable: true),
                    LabelsLabelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JunctionNotesLabels", x => x.JunctionId);
                    table.ForeignKey(
                        name: "FK_JunctionNotesLabels_DbNotes_NotesNoteId",
                        column: x => x.NotesNoteId,
                        principalTable: "DbNotes",
                        principalColumn: "NoteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JunctionNotesLabels_labelTable_LabelsLabelId",
                        column: x => x.LabelsLabelId,
                        principalTable: "labelTable",
                        principalColumn: "LabelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JunctionNotesLabels_LabelsLabelId",
                table: "JunctionNotesLabels",
                column: "LabelsLabelId");

            migrationBuilder.CreateIndex(
                name: "IX_JunctionNotesLabels_NotesNoteId",
                table: "JunctionNotesLabels",
                column: "NotesNoteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JunctionNotesLabels");
        }
    }
}
