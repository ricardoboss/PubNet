using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions.Store;
using PubNet.SDK.Services;

namespace PubNet.SDK.Tests;

public class ConcurrentRequestBlockingRequestAdapterTests
{
	private static RequestInformation Request(string path = "https://example.test/some-endpoint") => new()
	{
		HttpMethod = Method.GET,
		URI = new(path),
	};

	private static ConcurrentRequestBlockingRequestAdapter Adapter(IRequestAdapter inner) =>
		new(inner, NullLogger<ConcurrentRequestBlockingRequestAdapter>.Instance);

	[Test]
	public async Task TestOnlyOneRequestRunsAtATime()
	{
		const int requests = 12;

		var inner = new ConcurrencyTrackingRequestAdapter();
		using var adapter = Adapter(inner);

		// Task.Run so the calls genuinely race on the thread pool rather than interleaving on one thread
		var calls = Enumerable
			.Range(0, requests)
			.Select(_ => Task.Run(async () =>
				await adapter.SendAsync(Request(), TestModel.CreateFromDiscriminatorValue)))
			.ToArray();

		await Task.WhenAll(calls);

		Assert.Multiple(() =>
		{
			Assert.That(inner.MaxObservedConcurrency, Is.EqualTo(1),
				"requests must be serialized, but overlapping calls reached the inner adapter");
			Assert.That(inner.TotalCalls, Is.EqualTo(requests));
		});
	}

	[Test]
	public async Task TestLockIsReleasedWhenInnerAdapterThrows()
	{
		var inner = new ConcurrencyTrackingRequestAdapter { ThrowOnCall = 1 };
		using var adapter = Adapter(inner);

		Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await adapter.SendAsync(Request(), TestModel.CreateFromDiscriminatorValue));

		// would deadlock (or time out) if the failed request never released the lock
		var second = adapter.SendAsync(Request(), TestModel.CreateFromDiscriminatorValue);
		var completed = await Task.WhenAny(second, Task.Delay(TimeSpan.FromSeconds(5)));

		Assert.That(completed, Is.SameAs(second), "the lock was not released after the inner adapter threw");
		await second;
	}

	[Test]
	public async Task TestCancellationWhileWaitingDoesNotRunTheRequest()
	{
		var inner = new ConcurrencyTrackingRequestAdapter();
		using var adapter = Adapter(inner);

		// occupy the lock so the next caller has to wait
		inner.Gate = new(false);
		var holder = Task.Run(async () => await adapter.SendAsync(Request(), TestModel.CreateFromDiscriminatorValue));
		Assert.That(inner.Entered.Wait(TimeSpan.FromSeconds(5)), Is.True, "the first request never started");

		using var cts = new CancellationTokenSource();
		var blocked = adapter.SendAsync(Request(), TestModel.CreateFromDiscriminatorValue,
			cancellationToken: cts.Token);

		await cts.CancelAsync();

		Assert.CatchAsync<OperationCanceledException>(async () => await blocked);

		inner.Gate.Set();
		await holder;

		Assert.That(inner.TotalCalls, Is.EqualTo(1),
			"the cancelled request must not have been forwarded to the inner adapter");
	}

	private sealed class TestModel : IParsable
	{
		public static TestModel CreateFromDiscriminatorValue(IParseNode parseNode) => new();

		public IDictionary<string, Action<IParseNode>> GetFieldDeserializers() =>
			new Dictionary<string, Action<IParseNode>>();

		public void Serialize(ISerializationWriter writer)
		{
		}
	}

	/// <summary>
	/// Records how many calls are inside the adapter at the same time.
	/// </summary>
	private sealed class ConcurrencyTrackingRequestAdapter : IRequestAdapter
	{
		private int current;
		private int total;

		public int MaxObservedConcurrency { get; private set; }

		public int TotalCalls => total;

		/// <summary>When set, calls block on this until it is signalled.</summary>
		public ManualResetEventSlim? Gate { get; set; }

		/// <summary>Signalled once a call has entered.</summary>
		public ManualResetEventSlim Entered { get; } = new(false);

		/// <summary>1-based index of the call that should throw, if any.</summary>
		public int? ThrowOnCall { get; set; }

		private async Task<T?> TrackAsync<T>()
		{
			var callNumber = Interlocked.Increment(ref total);
			var running = Interlocked.Increment(ref current);

			lock (this)
			{
				if (running > MaxObservedConcurrency)
					MaxObservedConcurrency = running;
			}

			Entered.Set();

			try
			{
				// yield so that, if the lock were broken, overlapping calls would actually be observed
				await Task.Delay(15);

				Gate?.Wait();

				if (ThrowOnCall == callNumber)
					throw new InvalidOperationException("inner adapter failure");

				return default;
			}
			finally
			{
				Interlocked.Decrement(ref current);
			}
		}

		public Task<ModelType?> SendAsync<ModelType>(RequestInformation requestInfo, ParsableFactory<ModelType> factory,
			Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
			CancellationToken cancellationToken = default) where ModelType : IParsable => TrackAsync<ModelType>();

		public Task<IEnumerable<ModelType>?> SendCollectionAsync<ModelType>(RequestInformation requestInfo,
			ParsableFactory<ModelType> factory, Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
			CancellationToken cancellationToken = default) where ModelType : IParsable =>
			TrackAsync<IEnumerable<ModelType>>();

		public Task<ModelType?> SendPrimitiveAsync<ModelType>(RequestInformation requestInfo,
			Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
			CancellationToken cancellationToken = default) => TrackAsync<ModelType>();

		public Task<IEnumerable<ModelType>?> SendPrimitiveCollectionAsync<ModelType>(RequestInformation requestInfo,
			Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
			CancellationToken cancellationToken = default) => TrackAsync<IEnumerable<ModelType>>();

		public Task SendNoContentAsync(RequestInformation requestInfo,
			Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null,
			CancellationToken cancellationToken = default) => TrackAsync<object>();

		public Task<T?> ConvertToNativeRequestAsync<T>(RequestInformation requestInfo,
			CancellationToken cancellationToken = default) => Task.FromResult<T?>(default);

		public void EnableBackingStore(IBackingStoreFactory backingStoreFactory)
		{
		}

		public ISerializationWriterFactory SerializationWriterFactory =>
			throw new NotSupportedException();

		public string? BaseUrl { get; set; }
	}
}
