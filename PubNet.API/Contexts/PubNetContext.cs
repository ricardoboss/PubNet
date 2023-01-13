using Microsoft.EntityFrameworkCore;
using PubNet.API.Models;
using PubNet.Models;

namespace PubNet.API.Contexts;

public class PubNetContext : DbContext
{
    public DbSet<Package> Packages { get; set; }

    public DbSet<PackageVersion> PackageVersions { get; set; }

    public DbSet<Author> Authors { get; set; }

    public DbSet<AuthorToken> Tokens { get; set; }

    public DbSet<PendingArchive> PendingArchives { get; set; }

    public DbSet<PackageVersionAnalysis> PackageVersionAnalyses { get; set; }

    public PubNetContext(DbContextOptions<PubNetContext> options) : base(options)
    {
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Author>()
            .HasIndex(a => a.Email)
            .IsUnique();

        modelBuilder.Entity<Author>()
            .HasIndex(a => a.Username)
            .IsUnique();

        modelBuilder.Entity<AuthorToken>()
            .HasOne<Author>(nameof(AuthorToken.Owner))
            .WithMany(a => a.Tokens);

        modelBuilder.Entity<AuthorToken>()
            .HasIndex(a => a.Name);

        modelBuilder.Entity<AuthorToken>()
            .HasIndex(t => new { t.Name, t.OwnerId })
            .IsUnique();

        modelBuilder.Entity<Package>()
            .HasOne<Author>(nameof(Package.Author))
            .WithMany(a => a.Packages);

        modelBuilder.Entity<Package>()
            .HasIndex(p => p.Name)
            .IsUnique();

        modelBuilder.Entity<Package>()
            .Navigation(p => p.Latest)
            .AutoInclude();

        modelBuilder.Entity<Package>()
            .Navigation(p => p.Author)
            .AutoInclude();

        modelBuilder.Entity<PackageVersion>()
            .HasIndex(p => p.PublishedAtUtc)
            .IsDescending();

        modelBuilder.Entity<PackageVersion>()
            .HasIndex(p => p.Version);

        modelBuilder.Entity<PackageVersion>()
            .Property(v => v.Pubspec)
            .HasColumnType("json");

        modelBuilder.Entity<PackageVersionAnalysis>()
            .Navigation(a => a.Version)
            .AutoInclude();
    }
}