﻿using Microsoft.EntityFrameworkCore;
using PubNet.API.Models;
using PubNet.Models;

namespace PubNet.API.Contexts;

public class PubNetContext : DbContext
{
    public DbSet<Package> Packages { get; set; }

    public DbSet<PackageVersion> PackageVersions { get; set; }

    public DbSet<Author> Authors { get; set; }

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
