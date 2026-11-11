using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions.Store;
using Microsoft.Kiota.Serialization.Json;

namespace PubNet.SDK.Tests;

internal sealed class SuccessfulRequestAdapter : IRequestAdapter
{
    public Task<ModelType?> SendAsync<ModelType>(
        RequestInformation requestInfo,
        ParsableFactory<ModelType> factory,
        Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
        CancellationToken cancellationToken = default)
        where ModelType : IParsable
        => Task.FromResult<ModelType?>(default);

    public Task<IEnumerable<ModelType>?> SendCollectionAsync<ModelType>(
        RequestInformation requestInfo,
        ParsableFactory<ModelType> factory,
        Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
        CancellationToken cancellationToken = default)
        where ModelType : IParsable
        => Task.FromResult<IEnumerable<ModelType>?>(null);

    public Task<ModelType?> SendPrimitiveAsync<ModelType>(
        RequestInformation requestInfo,
        Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
        CancellationToken cancellationToken = default)
        => Task.FromResult<ModelType?>(default);

    public Task<IEnumerable<ModelType>?> SendPrimitiveCollectionAsync<ModelType>(
        RequestInformation requestInfo,
        Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
        CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<ModelType>?>(null);

    public Task SendNoContentAsync(
        RequestInformation requestInfo,
        Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task<T?> ConvertToNativeRequestAsync<T>(
        RequestInformation requestInfo,
        CancellationToken cancellationToken = default)
        => Task.FromResult<T?>(default);

    public void EnableBackingStore(IBackingStoreFactory backingStoreFactory)
    {
    }

    public ISerializationWriterFactory SerializationWriterFactory { get; } =
        new JsonSerializationWriterFactory();

    public string? BaseUrl { get; set; } = "https://example.test";
}

