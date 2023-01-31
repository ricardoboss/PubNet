using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PubNet.Database.Models;

namespace PubNet.Database;

public class PubNetContext : DbContext, IDesignTimeDbContextFactory<PubNetContext>
{
    public DbSet<Package> Packages { get; set; } = null!;

    public DbSet<PackageVersion> PackageVersions { get; set; } = null!;

    public DbSet<Author> Authors { get; set; } = null!;

    public DbSet<PendingArchive> PendingArchives { get; set; } = null!;

    public DbSet<PackageVersionAnalysis> PackageVersionAnalyses { get; set; } = null!;

    /// <inheritdoc />
    public PubNetContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PubNetContext>();
        optionsBuilder
            .UseNpgsql("Host=localhost;Database=pubnet;Username=pubnet;Password=pubnet")
            .ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))
        ;

        return new(optionsBuilder.Options);
    }

    public PubNetContext()
    {
    }

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
            .HasIndex(a => a.UserName)
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

        modelBuilder.Entity<PackageVersion>()
            .HasIndex(p => p.PublishedAtUtc)
            .IsDescending();

        modelBuilder.Entity<PackageVersion>()
            .HasOne<PackageVersionAnalysis>(p => p.Analysis)
            .WithOne(a => a.Version)
            .HasForeignKey<PackageVersionAnalysis>(a => a.VersionId)
            .IsRequired(false);

        modelBuilder.Entity<PackageVersion>()
            .HasIndex(p => p.Version);

        modelBuilder.Entity<PackageVersion>()
            .Property(v => v.PubSpec)
            .HasColumnType("json");

        modelBuilder.Entity<PackageVersionAnalysis>()
            .HasOne<PackageVersion>(a => a.Version)
            .WithOne(p => p.Analysis)
            .IsRequired();

        modelBuilder.Entity<PackageVersionAnalysis>()
            .Navigation(a => a.Version)
            .AutoInclude();

        modelBuilder.Entity<PendingArchive>()
            .Navigation(p => p.Uploader)
            .AutoInclude();
    }
}
