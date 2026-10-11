using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Razor.Migrations
{
    /// <inheritdoc />
    public partial class SyncDatabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Author_AuthorId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Post_ObservationId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Post_Proposal_ObservationId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Taxon_Proposal_TaxonId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Taxon_TaxonId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Taxon_Taxon_ParentId",
                table: "Taxon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Taxon",
                table: "Taxon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Post",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Author",
                table: "Author");

            migrationBuilder.RenameTable(
                name: "Taxon",
                newName: "taxons");

            migrationBuilder.RenameTable(
                name: "Post",
                newName: "posts");

            migrationBuilder.RenameTable(
                name: "Author",
                newName: "authors");

            migrationBuilder.RenameIndex(
                name: "IX_Taxon_ParentId",
                table: "taxons",
                newName: "IX_taxons_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_TaxonId",
                table: "posts",
                newName: "IX_posts_TaxonId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_Proposal_TaxonId",
                table: "posts",
                newName: "IX_posts_Proposal_TaxonId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_Proposal_ObservationId",
                table: "posts",
                newName: "IX_posts_Proposal_ObservationId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_ObservationId",
                table: "posts",
                newName: "IX_posts_ObservationId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_AuthorId",
                table: "posts",
                newName: "IX_posts_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_taxons",
                table: "taxons",
                column: "TaxonId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_posts",
                table: "posts",
                column: "PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_authors",
                table: "authors",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_posts_authors_AuthorId",
                table: "posts",
                column: "AuthorId",
                principalTable: "authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_posts_ObservationId",
                table: "posts",
                column: "ObservationId",
                principalTable: "posts",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_posts_Proposal_ObservationId",
                table: "posts",
                column: "Proposal_ObservationId",
                principalTable: "posts",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_taxons_Proposal_TaxonId",
                table: "posts",
                column: "Proposal_TaxonId",
                principalTable: "taxons",
                principalColumn: "TaxonId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_taxons_TaxonId",
                table: "posts",
                column: "TaxonId",
                principalTable: "taxons",
                principalColumn: "TaxonId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_taxons_taxons_ParentId",
                table: "taxons",
                column: "ParentId",
                principalTable: "taxons",
                principalColumn: "TaxonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_posts_authors_AuthorId",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_posts_ObservationId",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_posts_Proposal_ObservationId",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_taxons_Proposal_TaxonId",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_taxons_TaxonId",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "FK_taxons_taxons_ParentId",
                table: "taxons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_taxons",
                table: "taxons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_posts",
                table: "posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_authors",
                table: "authors");

            migrationBuilder.RenameTable(
                name: "taxons",
                newName: "Taxon");

            migrationBuilder.RenameTable(
                name: "posts",
                newName: "Post");

            migrationBuilder.RenameTable(
                name: "authors",
                newName: "Author");

            migrationBuilder.RenameIndex(
                name: "IX_taxons_ParentId",
                table: "Taxon",
                newName: "IX_Taxon_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_posts_TaxonId",
                table: "Post",
                newName: "IX_Post_TaxonId");

            migrationBuilder.RenameIndex(
                name: "IX_posts_Proposal_TaxonId",
                table: "Post",
                newName: "IX_Post_Proposal_TaxonId");

            migrationBuilder.RenameIndex(
                name: "IX_posts_Proposal_ObservationId",
                table: "Post",
                newName: "IX_Post_Proposal_ObservationId");

            migrationBuilder.RenameIndex(
                name: "IX_posts_ObservationId",
                table: "Post",
                newName: "IX_Post_ObservationId");

            migrationBuilder.RenameIndex(
                name: "IX_posts_AuthorId",
                table: "Post",
                newName: "IX_Post_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Taxon",
                table: "Taxon",
                column: "TaxonId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Post",
                table: "Post",
                column: "PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Author",
                table: "Author",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Author_AuthorId",
                table: "Post",
                column: "AuthorId",
                principalTable: "Author",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Post_ObservationId",
                table: "Post",
                column: "ObservationId",
                principalTable: "Post",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Post_Proposal_ObservationId",
                table: "Post",
                column: "Proposal_ObservationId",
                principalTable: "Post",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Taxon_Proposal_TaxonId",
                table: "Post",
                column: "Proposal_TaxonId",
                principalTable: "Taxon",
                principalColumn: "TaxonId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Taxon_TaxonId",
                table: "Post",
                column: "TaxonId",
                principalTable: "Taxon",
                principalColumn: "TaxonId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Taxon_Taxon_ParentId",
                table: "Taxon",
                column: "ParentId",
                principalTable: "Taxon",
                principalColumn: "TaxonId");
        }
    }
}
