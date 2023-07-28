using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearning__portal.Migrations
{
    /// <inheritdoc />
    public partial class addweekToUnitTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Week",
                table: "Notes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Week",
                table: "Notes");
        }
    }
}
