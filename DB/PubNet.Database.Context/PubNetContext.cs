using Microsoft.EntityFrameworkCore;
using PubNet.Database.Entities;
using PubNet.Database.Entities.Auth;
using PubNet.Database.Entities.Dart;
using PubNet.Database.Entities.Nuget;
using PubNet.Database.Entities.Packages;

namespace PubNet.Database.Context;

public class PubNetContext(DbContextOptions<PubNetContext> options) : DbContext(options)
{
	public DbSet<Author> Authors { get; init; } = null!;

	public DbSet<Identity> Identities { get; init; } = null!;

	#region Dart Packages

	public DbSet<DartPackage> DartPackages { get; init; } = null!;

	public DbSet<DartPackageVersion> DartPackageVersions { get; init; } = null!;

	public DbSet<DartPackageVersionArchive> DartPackageArchives { get; init; } = null!;

	public DbSet<DartPendingArchive> DartPendingArchives { get; init; } = null!;

	public DbSet<DartPackageVersionAnalysis> DartPackageVersionAnalyses { get; init; } = null!;

	#endregion

	#region Nuget Packages

	public DbSet<NugetPackage> NugetPackages { get; init; } = null!;

	public DbSet<NugetPackageVersion> NugetPackageVersions { get; init; } = null!;

	public DbSet<NugetPackageVersionArchive> NugetPackageArchives { get; init; } = null!;

	#endregion

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// can't use attribute because it gets inherited by child classes
		new PackageArchiveConfiguration().Configure(modelBuilder.Entity<PackageArchive>());
	}
}
