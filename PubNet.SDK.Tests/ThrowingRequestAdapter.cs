using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions.Store;
using Microsoft.Kiota.Serialization.Json;

namespace PubNet.SDK.Tests;

/// <summary>
/// A request adapter whose every send throws, so the services' error mapping can be exercised without a
/// server. Pass the exception the generated client would raise for the response being simulated.
/// </summary>
internal sealed class ThrowingRequestAdapter(Exception exception) : IRequestAdapter
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
	public ISerializationWriterFactory SerializationWriterFactory { get; } = new JsonSerializationWriterFactory();

	public string? BaseUrl { get; set; } = "https://example.test";
}
