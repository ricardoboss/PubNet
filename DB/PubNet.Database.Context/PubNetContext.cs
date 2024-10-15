using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PubNet.Database.Entities;
using PubNet.Database.Entities.Auth;
using PubNet.Database.Entities.Dart;
using PubNet.Database.Entities.Nuget;
using PubNet.Database.Entities.Packages;

namespace PubNet.Database.Context;

public class PubNetContext(DbContextOptions<PubNetContext> options) : DbContext(options)
{
	public DbSet<Author> Authors { get; init; } = null!;

	#region Authentication

	public DbSet<Identity> Identities { get; init; } = null!;

	public DbSet<Token> Tokens { get; init; } = null!;

	#endregion

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

	public static async Task RunMigrations(IServiceProvider serviceProvider)
	{
		serviceProvider.GetRequiredService<ILogger<PubNetContext>>().LogInformation("Migrating database");

		using var startupScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

		await startupScope.ServiceProvider.GetRequiredService<PubNetContext>().Database.MigrateAsync();
	}
}
