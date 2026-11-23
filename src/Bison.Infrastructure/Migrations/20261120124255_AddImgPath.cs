using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Infastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImgPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImgPath",
                table: "posts",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgPath",
                table: "posts");
        }
    }
}
