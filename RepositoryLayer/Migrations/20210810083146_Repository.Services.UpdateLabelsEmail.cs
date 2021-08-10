using Microsoft.EntityFrameworkCore.Migrations;

namespace RepositoryLayer.Migrations
{
    public partial class RepositoryServicesUpdateLabelsEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "labelTable",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "labelTable");
        }
    }
}
