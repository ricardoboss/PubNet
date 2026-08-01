using Microsoft.Extensions.Logging.Abstractions;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Generated.Models;
using PubNet.SDK.Services;

namespace PubNet.SDK.Tests;

public class CachingPackagesServiceTests
{
	private static (CachingPackagesService caching, Mock<IPackagesService> inner) Create()
	{
		var inner = new Mock<IPackagesService>();

		inner.Setup(s => s.GetPackageAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((string name, bool _, CancellationToken _) => new PackageDto { Name = name });

		inner.Setup(s => s.GetPackageVersionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((string _, string version, CancellationToken _) => new PackageVersionDto { Version = version });

		inner.Setup(s => s.GetPackagesByAuthorAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new AuthorPackagesResponseDto());

		return (new(inner.Object, NullLogger<CachingPackagesService>.Instance), inner);
	}

	[Test]
	public async Task TestCachesReadsUntilSomethingChanges()
	{
		var (caching, inner) = Create();

		await caching.GetPackageAsync("pkg", false);
		await caching.GetPackageAsync("pkg", false);

		inner.Verify(s => s.GetPackageAsync("pkg", false, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Test]
	public async Task TestDeletingPackageDropsItsCachedVersions()
	{
		var (caching, inner) = Create();

		await caching.GetPackageVersionAsync("pkg", "1.0.0");
		await caching.DeletePackageAsync("pkg");
		await caching.GetPackageVersionAsync("pkg", "1.0.0");

		// the version of a deleted package must not be served from the cache
		inner.Verify(s => s.GetPackageVersionAsync("pkg", "1.0.0", It.IsAny<CancellationToken>()), Times.Exactly(2));
	}

	[Test]
	public async Task TestDeletingOneVersionKeepsTheOthersCached()
	{
		var (caching, inner) = Create();

		await caching.GetPackageVersionAsync("pkg", "1.0.0");
		await caching.GetPackageVersionAsync("pkg", "2.0.0");

		await caching.DeletePackageVersionAsync("pkg", "1.0.0");

		await caching.GetPackageVersionAsync("pkg", "1.0.0");
		await caching.GetPackageVersionAsync("pkg", "2.0.0");

		Assert.Multiple(() =>
		{
			inner.Verify(s => s.GetPackageVersionAsync("pkg", "1.0.0", It.IsAny<CancellationToken>()),
				Times.Exactly(2));
			inner.Verify(s => s.GetPackageVersionAsync("pkg", "2.0.0", It.IsAny<CancellationToken>()),
				Times.Once);
		});
	}

	[Test]
	[TestCaseSource(nameof(MutatingCalls))]
	public async Task TestMutationsDropCachedAuthorPackages(
		string label, Func<IPackagesService, Task> mutate)
	{
		var (caching, inner) = Create();

		await caching.GetPackagesByAuthorAsync("ricardoboss");
		await mutate(caching);
		await caching.GetPackagesByAuthorAsync("ricardoboss");

		inner.Verify(s => s.GetPackagesByAuthorAsync("ricardoboss", It.IsAny<CancellationToken>()),
			Times.Exactly(2), $"author packages were still cached after {label}");
	}

	private static IEnumerable<TestCaseData> MutatingCalls()
	{
		yield return new("delete package", (IPackagesService c) => c.DeletePackageAsync("pkg"));
		yield return new("delete version", (IPackagesService c) => c.DeletePackageVersionAsync("pkg", "1.0.0"));
		yield return new("discontinue", (IPackagesService c) => c.DiscontinuePackageAsync("pkg", null));
		yield return new("retract version", (IPackagesService c) => c.RetractPackageVersionAsync("pkg", "1.0.0"));
	}

	[Test]
	[TestCaseSource(nameof(MutatingCalls))]
	public async Task TestMutationsDropTheCachedPackage(string label, Func<IPackagesService, Task> mutate)
	{
		var (caching, inner) = Create();

		await caching.GetPackageAsync("pkg", true);
		await caching.GetPackageAsync("pkg", false);

		await mutate(caching);

		await caching.GetPackageAsync("pkg", true);
		await caching.GetPackageAsync("pkg", false);

		Assert.Multiple(() =>
		{
			inner.Verify(s => s.GetPackageAsync("pkg", true, It.IsAny<CancellationToken>()),
				Times.Exactly(2), $"package (with author) was still cached after {label}");
			inner.Verify(s => s.GetPackageAsync("pkg", false, It.IsAny<CancellationToken>()),
				Times.Exactly(2), $"package (without author) was still cached after {label}");
		});
	}
}
