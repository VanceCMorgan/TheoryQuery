using Microsoft.EntityFrameworkCore.Migrations;

namespace Capstone2019.Migrations
{
    public partial class recordCompLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rec_ID",
                table: "RecTags",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Comp_ID",
                table: "CompTags",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rec_ID",
                table: "RecTags");

            migrationBuilder.DropColumn(
                name: "Comp_ID",
                table: "CompTags");
        }
    }
}
