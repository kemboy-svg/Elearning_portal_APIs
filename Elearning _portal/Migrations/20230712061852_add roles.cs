using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearning__portal.Migrations
{
    /// <inheritdoc />
    public partial class addroles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "test",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "reg_no",
                table: "Students",
                newName: "Reg_no");

            migrationBuilder.RenameColumn(
                name: "enrolled_unit",
                table: "Students",
                newName: "fullName");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Course",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Course",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "Reg_no",
                table: "Students",
                newName: "reg_no");

            migrationBuilder.RenameColumn(
                name: "fullName",
                table: "Students",
                newName: "enrolled_unit");

            migrationBuilder.AddColumn<DateTime>(
                name: "date",
                table: "Students",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "test",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
