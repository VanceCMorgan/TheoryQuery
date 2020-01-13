using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Capstone2019.Migrations
{
    public partial class lastSign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Owner_ID",
                table: "Favourites");
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Last_Sign_In",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "Owner_ID",
                table: "Favourites",
                nullable: false,
                defaultValue: 0);
        }
    }
}
