using System.Diagnostics.CodeAnalysis;

namespace PubNet.Database.Entities.Packages;

public abstract class BasePackageVersion<
	[DynamicallyAccessedMembers(AotHelper.DynamicallyAccessedMemberTypes)] TPackage> where TPackage : class
{
	public Guid Id { get; init; }

	public Guid PackageId { get; init; }

	public virtual TPackage Package { get; set; } = null!;

	public string Version { get; init; } = null!;

	public DateTimeOffset PublishedAt { get; init; }
}
