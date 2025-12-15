using Microsoft.Extensions.Logging;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions.Store;

namespace PubNet.SDK.Services;

internal sealed class ConcurrentRequestBlockingRequestAdapter(IRequestAdapter innerAdapter, ILogger<ConcurrentRequestBlockingRequestAdapter> logger) : IRequestAdapter, IDisposable
{
	public void EnableBackingStore(IBackingStoreFactory backingStoreFactory) =>
		innerAdapter.EnableBackingStore(backingStoreFactory);

	public ISerializationWriterFactory SerializationWriterFactory => innerAdapter.SerializationWriterFactory;

	public string? BaseUrl
	{
		get => innerAdapter.BaseUrl;
		set => innerAdapter.BaseUrl = value;
	}

	private readonly ManualResetEventSlim blockFlag = new();

	private async Task<IDisposable?> BlockUntilFree(RequestInformation requestInfo, CancellationToken cancellationToken)
	{
		if (blockFlag.IsSet)
		{
			logger.LogTrace("Request {RequestPath} blocked, waiting until free...", requestInfo.UrlTemplate);

			await WaitUntilFree(cancellationToken);
		}

		if (cancellationToken.IsCancellationRequested)
			return null;

		blockFlag.Set();

		logger.LogTrace("Request {RequestPath} acquired the lock", requestInfo.UrlTemplate);

		return new RequestBlocker(() =>
		{
			blockFlag.Reset();

			logger.LogTrace("Request {RequestPath} freed the lock", requestInfo.UrlTemplate);
		});
	}

	private async Task WaitUntilFree(CancellationToken cancellationToken)
	{
		try
		{
			do
			{
				await Task.Delay(100, cancellationToken);
			} while (blockFlag.IsSet);
		}
		catch (Exception)
		{
			if (cancellationToken.IsCancellationRequested)
				return;

			throw;
		}
	}

	public async Task<ModelType?> SendAsync<ModelType>(RequestInformation requestInfo,
		ParsableFactory<ModelType> factory,
		Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
		CancellationToken cancellationToken = default) where ModelType : IParsable
	{
		using var _ = await BlockUntilFree(requestInfo, cancellationToken);

		return await innerAdapter.SendAsync(requestInfo, factory, errorMapping, cancellationToken);
	}

	public async Task<IEnumerable<ModelType>?> SendCollectionAsync<ModelType>(RequestInformation requestInfo,
		ParsableFactory<ModelType> factory, Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
		CancellationToken cancellationToken = default) where ModelType : IParsable
	{
		using var _ = await BlockUntilFree(requestInfo, cancellationToken);

		return await innerAdapter.SendCollectionAsync(requestInfo, factory, errorMapping, cancellationToken);
	}

	public async Task<ModelType?> SendPrimitiveAsync<ModelType>(RequestInformation requestInfo,
		Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
		CancellationToken cancellationToken = default)
	{
		using var _ = await BlockUntilFree(requestInfo, cancellationToken);

		return await innerAdapter.SendPrimitiveAsync<ModelType>(requestInfo, errorMapping, cancellationToken);
	}

	public async Task<IEnumerable<ModelType>?> SendPrimitiveCollectionAsync<ModelType>(RequestInformation requestInfo,
		Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
		CancellationToken cancellationToken = default)
	{
		using var _ = await BlockUntilFree(requestInfo, cancellationToken);

		return await innerAdapter.SendPrimitiveCollectionAsync<ModelType>(requestInfo, errorMapping, cancellationToken);
	}

	public async Task SendNoContentAsync(RequestInformation requestInfo,
		Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
		CancellationToken cancellationToken = default)
	{
		using var _ = await BlockUntilFree(requestInfo, cancellationToken);

		await innerAdapter.SendNoContentAsync(requestInfo, errorMapping, cancellationToken);
	}

	public async Task<T?> ConvertToNativeRequestAsync<T>(RequestInformation requestInfo,
		CancellationToken cancellationToken = default) =>
		await innerAdapter.ConvertToNativeRequestAsync<T>(requestInfo, cancellationToken);

	public void Dispose()
	{
		blockFlag.Dispose();
	}
}

file class RequestBlocker(Action unblock) : IDisposable
{
	public void Dispose() => unblock();
}
