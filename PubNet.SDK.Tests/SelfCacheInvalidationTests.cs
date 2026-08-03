using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Serialization.Json;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Models;
using PubNet.SDK.Services;

namespace PubNet.SDK.Tests;

/// <summary>
/// The authenticated author is cached inside <see cref="ApiAuthenticationService"/>, separately from the author
/// cache, so editing a profile has to invalidate it explicitly or the navbar keeps showing the old data.
/// </summary>
public class SelfCacheInvalidationTests
{
	private static Mock<IRequestAdapter> Adapter()
	{
		var adapter = new Mock<IRequestAdapter>();

		adapter.SetupProperty(a => a.BaseUrl, "https://example.test");

		// request bodies are serialized before the adapter is called, so this cannot stay null
		adapter.SetupGet(a => a.SerializationWriterFactory).Returns(new JsonSerializationWriterFactory());

		adapter
			.Setup(a => a.SendAsync(
				It.IsAny<RequestInformation>(),
				It.IsAny<ParsableFactory<AuthorDto>>(),
				It.IsAny<Dictionary<string, ParsableFactory<IParsable>>>(),
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(new AuthorDto { UserName = "ricardoboss" });

		return adapter;
	}

	private static Mock<ILoginTokenStorage> TokenStorage()
	{
		var storage = new Mock<ILoginTokenStorage>();

		storage.Setup(s => s.GetTokenAsync(It.IsAny<CancellationToken>())).ReturnsAsync("token");

		return storage;
	}

	private static void VerifySendCount(Mock<IRequestAdapter> adapter, int expected, string because)
	{
		adapter.Verify(a => a.SendAsync(
			It.IsAny<RequestInformation>(),
			It.IsAny<ParsableFactory<AuthorDto>>(),
			It.IsAny<Dictionary<string, ParsableFactory<IParsable>>>(),
			It.IsAny<CancellationToken>()), Times.Exactly(expected), because);
	}

	[Test]
	public async Task TestSelfIsCachedUntilInvalidated()
	{
		var adapter = Adapter();
		var client = new PubNetApiClient(adapter.Object);
		var auth = new ApiAuthenticationService(client, TokenStorage().Object,
			NullLogger<ApiAuthenticationService>.Instance);

		await auth.GetSelfAsync();
		await auth.GetSelfAsync();

		VerifySendCount(adapter, 1, "the second call should have come from the cache");

		auth.InvalidateSelf();

		await auth.GetSelfAsync();

		VerifySendCount(adapter, 2, "InvalidateSelf should have forced a reload");
	}

	[Test]
	public async Task TestUpdatingAnAuthorInvalidatesSelf()
	{
		var adapter = Adapter();
		var client = new PubNetApiClient(adapter.Object);
		var auth = new ApiAuthenticationService(client, TokenStorage().Object,
			NullLogger<ApiAuthenticationService>.Instance);
		var authors = new ApiAuthorService(client, auth);

		await auth.GetSelfAsync();

		await authors.UpdateAuthorAsync("ricardoboss", new());

		await auth.GetSelfAsync();

		// self GET, PATCH, self GET again - a stale cache would leave this at 2
		VerifySendCount(adapter, 3, "editing a profile should have dropped the cached self");
	}

	[Test]
	public async Task TestDeletingAnAuthorInvalidatesSelf()
	{
		var adapter = Adapter();
		var client = new PubNetApiClient(adapter.Object);
		var auth = new ApiAuthenticationService(client, TokenStorage().Object,
			NullLogger<ApiAuthenticationService>.Instance);
		var authors = new ApiAuthorService(client, auth);

		await auth.GetSelfAsync();

		await authors.DeleteAuthorAsync("ricardoboss", new());

		await auth.GetSelfAsync();

		VerifySendCount(adapter, 2, "deleting an author should have dropped the cached self");
	}
}
