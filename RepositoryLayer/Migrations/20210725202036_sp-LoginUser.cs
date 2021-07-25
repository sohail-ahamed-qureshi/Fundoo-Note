using Microsoft.EntityFrameworkCore.Migrations;

namespace RepositoryLayer.Migrations
{
    public partial class spLoginUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE PROCEDURE [dbo].[LoginUser]
                        @UserName varchar(255),
                        @Password varchar(255)
                    AS
                    BEGIN
                        select * from Users where UserName like @UserName and password like @Password
                    END";
            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
