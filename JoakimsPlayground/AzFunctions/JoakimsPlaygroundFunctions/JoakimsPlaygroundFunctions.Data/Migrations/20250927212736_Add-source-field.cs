using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JoakimsPlaygroundFunctions.Data.Migrations
{
    /// <inheritdoc />
    public partial class Addsourcefield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                schema: "rando",
                table: "Letters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                schema: "rando",
                table: "Letters");
        }
    }
}
