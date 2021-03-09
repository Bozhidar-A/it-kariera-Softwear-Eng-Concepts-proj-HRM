using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HotelReservationsManager.Data.Migrations
{
    public partial class AddedApplicationUserFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EGN",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "fireDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "firstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "hireDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "lastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "middleName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EGN",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "fireDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "firstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "hireDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "lastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "middleName",
                table: "AspNetUsers");
        }
    }
}
