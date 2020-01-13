using Microsoft.EntityFrameworkCore.Migrations;

namespace Capstone2019.Migrations
{
    public partial class OwnerID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Owner_ID",
                table: "Favourites");
        }
    }
}
