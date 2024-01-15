using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PubNet.Database.Models;

namespace PubNet.Database;

public class PubNetContext : DbContext, IDesignTimeDbContextFactory<PubNetContext>
{
	public PubNetContext()
	{
	}

	public PubNetContext(DbContextOptions<PubNetContext> options) : base(options)
	{
	}

	public static async Task RunMigrations(IServiceProvider serviceProvider)
	{
		serviceProvider.GetRequiredService<ILogger<PubNetContext>>().LogInformation("Migrating database");

		using var startupScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

		await startupScope.ServiceProvider.GetRequiredService<PubNetContext>().Database.MigrateAsync();
	}

	public DbSet<DartPackage> Packages { get; set; } = null!;

	public DbSet<DartPackageVersion> PackageVersions { get; set; } = null!;

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

		modelBuilder.Entity<DartPackage>()
			.HasOne<Author>(nameof(DartPackage.Author))
			.WithMany(a => a.DartPackages);

		modelBuilder.Entity<DartPackage>()
			.HasIndex(p => p.Name)
			.IsUnique();

		modelBuilder.Entity<DartPackage>()
			.Navigation(p => p.Latest)
			.AutoInclude();

		modelBuilder.Entity<DartPackageVersion>()
			.HasIndex(p => p.PublishedAtUtc)
			.IsDescending();

		modelBuilder.Entity<DartPackageVersion>()
			.HasOne<PackageVersionAnalysis>(p => p.Analysis)
			.WithOne(a => a.Version)
			.HasForeignKey<PackageVersionAnalysis>(a => a.VersionId)
			.IsRequired(false);

		modelBuilder.Entity<DartPackageVersion>()
			.HasIndex(p => p.Version);

		modelBuilder.Entity<DartPackageVersion>()
			.Property(v => v.PubSpec)
			.HasColumnType("json");

		modelBuilder.Entity<PackageVersionAnalysis>()
			.HasOne<DartPackageVersion>(a => a.Version)
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
