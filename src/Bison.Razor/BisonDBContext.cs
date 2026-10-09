using Bison.Razor.Models;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Bison.Razor;

public class BisonDBContext : DbContext
{ 
    public BisonDBContext(DbContextOptions<BisonDBContext> options) : base(options){    
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Author>().HasMany<Post>(a => a.Posts);
        modelBuilder.Entity<Comment>().HasOne<Observation>(c => c.Observation);
        modelBuilder.Entity<Observation>().HasOne<Taxon>(o => o.Taxon);
        modelBuilder.Entity<Observation>().HasMany<Comment>(o => o.Comments);
        modelBuilder.Entity<Observation>().HasMany<Proposal>(o => o.Proposals);
        modelBuilder.Entity<Post>().HasOne<Author>(p => p.Author);
        modelBuilder.Entity<Proposal>().HasOne<Taxon>(p => p.Taxon);
        modelBuilder.Entity<Proposal>().HasOne<Observation>(p => p.Observation);
        modelBuilder.Entity<Taxon>().HasMany<Taxon>(t => t.Children);
        modelBuilder.Entity<Taxon>().HasOne<Taxon>(t => t.Parent);
    }
}