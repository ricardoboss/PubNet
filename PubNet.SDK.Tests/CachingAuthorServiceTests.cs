using Microsoft.Extensions.Logging.Abstractions;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Generated.Models;
using PubNet.SDK.Services;

namespace PubNet.SDK.Tests;

public class CachingAuthorServiceTests
{
	private static (CachingAuthorService caching, Mock<IAuthorService> inner) Create()
	{
		var inner = new Mock<IAuthorService>();

		inner.Setup(s => s.GetAuthorAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((string username, CancellationToken _) => new AuthorDto { UserName = username });

		inner.Setup(s => s.GetAuthorsAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new AuthorsResponseDto());

		return (new(inner.Object, NullLogger<CachingAuthorService>.Instance), inner);
	}

	[Test]
	public async Task TestCachesAuthorUntilSomethingChanges()
	{
		var (caching, inner) = Create();

		await caching.GetAuthorAsync("ricardoboss");
		await caching.GetAuthorAsync("ricardoboss");

		inner.Verify(s => s.GetAuthorAsync("ricardoboss", It.IsAny<CancellationToken>()), Times.Once);
	}

	[Test]
	public async Task TestUpdateDropsTheCachedAuthor()
	{
		var (caching, inner) = Create();

		await caching.GetAuthorAsync("ricardoboss");
		await caching.UpdateAuthorAsync("ricardoboss", new());
		await caching.GetAuthorAsync("ricardoboss");

		inner.Verify(s => s.GetAuthorAsync("ricardoboss", It.IsAny<CancellationToken>()), Times.Exactly(2));
	}

	[Test]
	public async Task TestUpdateDropsTheCachedAuthorList()
	{
		var (caching, inner) = Create();

		await caching.GetAuthorsAsync();
		await caching.UpdateAuthorAsync("ricardoboss", new());
		await caching.GetAuthorsAsync();

		inner.Verify(s => s.GetAuthorsAsync(It.IsAny<CancellationToken>()),
			Times.Exactly(2), "the author list still held the pre-update data");
	}

	[Test]
	public async Task TestDeleteDropsTheCachedAuthorList()
	{
		var (caching, inner) = Create();

		await caching.GetAuthorsAsync();
		await caching.DeleteAuthorAsync("ricardoboss", new());
		await caching.GetAuthorsAsync();

		inner.Verify(s => s.GetAuthorsAsync(It.IsAny<CancellationToken>()),
			Times.Exactly(2), "the author list still held the deleted author");
	}

	[Test]
	public async Task TestUpdatingAnUncachedAuthorStillDropsTheList()
	{
		var (caching, inner) = Create();

		await caching.GetAuthorsAsync();

		// the author was never fetched individually, so the old guard returned before invalidating anything
		await caching.UpdateAuthorAsync("someone-else", new());

		await caching.GetAuthorsAsync();

		inner.Verify(s => s.GetAuthorsAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
	}
}
