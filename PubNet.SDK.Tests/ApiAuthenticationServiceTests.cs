using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions.Store;
using Microsoft.Kiota.Serialization.Json;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Exceptions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Models;
using PubNet.SDK.Services;

namespace PubNet.SDK.Tests;

public class ApiAuthenticationServiceTests
{
	private static ApiAuthenticationService Service(Exception thrownByTheApi)
	{
		var adapter = new ThrowingRequestAdapter(thrownByTheApi);

		return new(new PubNetApiClient(adapter), new Mock<ILoginTokenStorage>().Object);
	}

	// The generated error DTOs never escape the SDK. Callers see PubNetSdkException subclasses instead, so
	// catching the DTOs further up (as the login form used to) is dead code.
	[Test]
	public void TestLoginMapsEmailNotFoundToInvalidCredentials()
	{
		var api = new EmailNotFoundErrorDto { Error = new() { Code = "email-not-found", Message = "nope" } };

		var e = Assert.ThrowsAsync<InvalidLoginCredentialsException>(
			() => Service(api).LoginAsync("someone@example.test", "hunter2"));

		Assert.Multiple(() =>
		{
			Assert.That(e?.InnerException, Is.SameAs(api));
			// deployments are internal, so the message deliberately says which of the two it was
			Assert.That(e?.Message, Does.Contain("someone@example.test"));
		});
	}

	[Test]
	public void TestLoginMapsInvalidPasswordToInvalidCredentials()
	{
		var api = new InvalidPasswordErrorDto { Error = new() { Code = "invalid-password", Message = "nope" } };

		var e = Assert.ThrowsAsync<InvalidLoginCredentialsException>(
			() => Service(api).LoginAsync("someone@example.test", "hunter2"));

		Assert.Multiple(() =>
		{
			Assert.That(e?.InnerException, Is.SameAs(api));
			Assert.That(e?.Message, Does.Not.Contain("someone@example.test"));
		});
	}

	[Test]
	public void TestLoginMapsUnexpectedApiErrorsToUnexpectedResponse()
	{
		Assert.ThrowsAsync<UnexpectedResponseException>(
			() => Service(new ApiException("boom")).LoginAsync("someone@example.test", "hunter2"));
	}

	private sealed class ThrowingRequestAdapter(Exception exception) : IRequestAdapter
	{
		public Task<ModelType?> SendAsync<ModelType>(RequestInformation requestInfo, ParsableFactory<ModelType> factory,
			Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
			CancellationToken cancellationToken = default) where ModelType : IParsable => throw exception;

		public Task<IEnumerable<ModelType>?> SendCollectionAsync<ModelType>(RequestInformation requestInfo,
			ParsableFactory<ModelType> factory, Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
			CancellationToken cancellationToken = default) where ModelType : IParsable => throw exception;

		public Task<ModelType?> SendPrimitiveAsync<ModelType>(RequestInformation requestInfo,
			Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
			CancellationToken cancellationToken = default) => throw exception;

		public Task<IEnumerable<ModelType>?> SendPrimitiveCollectionAsync<ModelType>(RequestInformation requestInfo,
			Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
			CancellationToken cancellationToken = default) => throw exception;

		public Task SendNoContentAsync(RequestInformation requestInfo,
			Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
			CancellationToken cancellationToken = default) => throw exception;

		public Task<T?> ConvertToNativeRequestAsync<T>(RequestInformation requestInfo,
			CancellationToken cancellationToken = default) => throw exception;

		public void EnableBackingStore(IBackingStoreFactory backingStoreFactory)
		{
		}

		// the request builder serialises the body before it ever reaches SendAsync
		public ISerializationWriterFactory SerializationWriterFactory { get; } =
			new JsonSerializationWriterFactory();

		public string? BaseUrl { get; set; } = "https://example.test";
	}
}
