using Microsoft.EntityFrameworkCore;
using PubNet.Database;
using PubNet.Database.Models;

namespace PubNet.API.Tests;

/// <summary>
/// The production model relies on Npgsql's JSON support, which the in-memory provider does not have.
/// None of the settings or onboarding logic touches package versions, so the property is dropped here.
/// </summary>
internal sealed class TestPubNetContext(DbContextOptions<PubNetContext> options) : PubNetContext(options)
{
	/// <inheritdoc />
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<PackageVersion>().Ignore(v => v.PubSpec);
	}
}
