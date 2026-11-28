using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Infastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddObservationLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "posts",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "posts");
        }
    }
}
