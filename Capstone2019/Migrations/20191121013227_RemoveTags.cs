using Microsoft.EntityFrameworkCore.Migrations;

namespace Capstone2019.Migrations
{
    public partial class RemoveTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Records");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Records",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Compositions",
                nullable: false,
                defaultValue: "");
        }
    }
}
