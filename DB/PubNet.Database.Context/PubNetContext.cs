using Microsoft.EntityFrameworkCore;
using PubNet.Database.Entities;
using PubNet.Database.Entities.Auth;
using PubNet.Database.Entities.Dart;
using PubNet.Database.Entities.Nuget;

namespace PubNet.Database.Context;

public class PubNetContext(DbContextOptions<PubNetContext> options) : DbContext(options)
{
	public DbSet<Author> Authors { get; init; } = null!;

	public DbSet<Identity> Identities { get; init; } = null!;

	public DbSet<DartPackage> DartPackages { get; init; } = null!;

	public DbSet<DartPackageVersion> DartPackageVersions { get; init; } = null!;

	public DbSet<NugetPackage> NugetPackages { get; init; } = null!;

	public DbSet<NugetPackageVersion> NugetPackageVersions { get; init; } = null!;
}
