using Microsoft.EntityFrameworkCore;
using PubNet.Database.Entities.Auth;
using PubNet.Database.Entities.Dart;
using PubNet.Database.Entities.Nuget;

namespace PubNet.Database.Entities;

/// <summary>
/// The author entity represents a person who publishes and manages packages.
/// </summary>
[EntityTypeConfiguration<AuthorConfiguration, Author>]
public class Author
{
	public Guid Id { get; init; }

	public string UserName { get; init; } = null!;

	public DateTimeOffset RegisteredAtUtc { get; init; }

	public virtual Identity? Identity { get; init; }

	public virtual ICollection<DartPackage> DartPackages { get; init; } = null!;

	public virtual ICollection<NugetPackage> NugetPackages { get; init; } = null!;
}
