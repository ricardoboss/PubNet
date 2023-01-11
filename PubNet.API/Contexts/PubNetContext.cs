using Microsoft.EntityFrameworkCore;
using PubNet.API.Models;

namespace PubNet.API.Contexts;

public class PubNetContext : DbContext
{
    public DbSet<Package> Packages { get; set; }

    public DbSet<Author> Authors { get; set; }

    public DbSet<AuthorToken> Tokens { get; set; }

    public PubNetContext(DbContextOptions<PubNetContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AuthorToken>().HasOne(t => t.Owner).WithMany(a  => a .Tokens);
        modelBuilder.Entity<Package>().HasOne(p => p.Author).WithMany(a => a.Packages);
        modelBuilder.Entity<Package>().HasMany(p => p.Versions);
    }
}