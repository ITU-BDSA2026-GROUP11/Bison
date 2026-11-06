
using Bison.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bison.Infastructure;

public class BisonDBContext : IdentityDbContext<Author, IdentityRole<int>, int>
{
    public BisonDBContext(DbContextOptions<BisonDBContext> options) : base(options) { }

    // Existing database tables
    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<Observation> Observations { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Proposal> Proposals { get; set; } = null!;
    public DbSet<Taxon> Taxons { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Preserve existing table names
        modelBuilder.Entity<Author>().ToTable("authors");
        modelBuilder.Entity<Post>().ToTable("posts");
        modelBuilder.Entity<Taxon>().ToTable("taxons");

        // Store Identity's Id in the existing AuthorId column
        modelBuilder.Entity<Author>().Property(a => a.Id).HasColumnName("AuthorId");

        // Preserve existing Taxon column names
        modelBuilder.Entity<Taxon>().Property(t => t.dwc_TaxonID).HasColumnName("DwcTaxonId");
        modelBuilder.Entity<Taxon>().Property(t => t.VernacularName).HasColumnName("DanishVernacularName").IsRequired(false);

        // Post belongs to an Author
        modelBuilder.Entity<Post>().HasOne(p => p.Author).WithMany(a => a.Posts).HasForeignKey(p => p.AuthorId);

        // Taxon hierarchy
        modelBuilder.Entity<Taxon>().HasOne(t => t.Parent).WithMany(t => t.Children).HasForeignKey(t => t.ParentId);

        // Observation belongs to a Taxon
        modelBuilder.Entity<Observation>().HasOne(o => o.Taxon).WithMany().HasForeignKey(o => o.TaxonId);

        // Comment belongs to an Observation
        modelBuilder.Entity<Comment>().HasOne(c => c.Observation).WithMany(o => o.Comments).HasForeignKey(c => c.ObservationId);

        // Proposal belongs to an Observation
        modelBuilder.Entity<Proposal>().HasOne(p => p.Observation).WithMany(o => o.Proposals).HasForeignKey(p => p.ObservationId);

        // Proposal belongs to a Taxon
        modelBuilder.Entity<Proposal>().HasOne(p => p.Taxon).WithMany().HasForeignKey(p => p.TaxonId);

        // Preserve column names from previous migrations
        modelBuilder.Entity<Proposal>().Property(p => p.ObservationId).HasColumnName("Proposal_ObservationId");

        modelBuilder.Entity<Proposal>().Property(p => p.TaxonId).HasColumnName("Proposal_TaxonId");

        // Require user email to not be null
        modelBuilder.Entity<Author>().Property(a => a.Email).IsRequired();
    }
}
