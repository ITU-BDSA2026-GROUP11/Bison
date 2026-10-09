using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Razor.Migrations
{
    /// <inheritdoc />
    public partial class InitialDBSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Author",
                columns: table => new
                {
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author", x => x.AuthorId);
                });

            migrationBuilder.CreateTable(
                name: "Taxon",
                columns: table => new
                {
                    TaxonId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DwcTaxonId = table.Column<string>(type: "TEXT", nullable: false),
                    DanishVernacularName = table.Column<string>(type: "TEXT", nullable: false),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taxon", x => x.TaxonId);
                    table.ForeignKey(
                        name: "FK_Taxon_Taxon_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Taxon",
                        principalColumn: "TaxonId");
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    ObservationId = table.Column<int>(type: "INTEGER", nullable: true),
                    TaxonId = table.Column<int>(type: "INTEGER", nullable: true),
                    Proposal_ObservationId = table.Column<int>(type: "INTEGER", nullable: true),
                    Proposal_TaxonId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Post_Author_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Author",
                        principalColumn: "AuthorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_Post_ObservationId",
                        column: x => x.ObservationId,
                        principalTable: "Post",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_Post_Proposal_ObservationId",
                        column: x => x.Proposal_ObservationId,
                        principalTable: "Post",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_Taxon_Proposal_TaxonId",
                        column: x => x.Proposal_TaxonId,
                        principalTable: "Taxon",
                        principalColumn: "TaxonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_Taxon_TaxonId",
                        column: x => x.TaxonId,
                        principalTable: "Taxon",
                        principalColumn: "TaxonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_AuthorId",
                table: "Post",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ObservationId",
                table: "Post",
                column: "ObservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Proposal_ObservationId",
                table: "Post",
                column: "Proposal_ObservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Proposal_TaxonId",
                table: "Post",
                column: "Proposal_TaxonId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_TaxonId",
                table: "Post",
                column: "TaxonId");

            migrationBuilder.CreateIndex(
                name: "IX_Taxon_ParentId",
                table: "Taxon",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "Author");

            migrationBuilder.DropTable(
                name: "Taxon");
        }
    }
}
