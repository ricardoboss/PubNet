using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Extensions;
using PubNet.SDK.Generated;

namespace PubNet.SDK.Tests;

public class ServiceCollectionExtensionsTests
{
	private static readonly Uri BaseAddress = new("https://pubnet.example.test/api/");

	private static ServiceProvider BuildProvider()
	{
		var services = new ServiceCollection();

		services.AddLogging();
		services.AddPubNetApiServices<FakeTokenStorage>(BaseAddress);

		return services.BuildServiceProvider();
	}

	[Test]
	public void TestConfiguresItsOwnNamedClient()
	{
		using var provider = BuildProvider();

		var client = provider.GetRequiredService<IHttpClientFactory>()
			.CreateClient(PubNet.SDK.Extensions.ServiceCollectionExtensions.HttpClientName);

		Assert.Multiple(() =>
		{
			Assert.That(client.BaseAddress, Is.EqualTo(BaseAddress));
			Assert.That(client.DefaultRequestHeaders.UserAgent.ToString(), Does.Contain("PubNet.SDK"));
		});
	}

	// The SDK used to resolve the default HttpClient and reconfigure it. That client belongs to the
	// consuming application, so it has to come back untouched.
	[Test]
	public void TestLeavesTheDefaultClientAlone()
	{
		using var provider = BuildProvider();

		var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient();

		Assert.Multiple(() =>
		{
			Assert.That(client.BaseAddress, Is.Null);
			Assert.That(client.DefaultRequestHeaders.UserAgent, Is.Empty);
		});
	}

	[Test]
	public void TestResolvesTheGeneratedClient()
	{
		using var provider = BuildProvider();
		using var scope = provider.CreateScope();

		Assert.Multiple(() =>
		{
			Assert.That(scope.ServiceProvider.GetRequiredService<PubNetApiClient>(), Is.Not.Null);
			Assert.That(scope.ServiceProvider.GetRequiredService<IRequestAdapter>(), Is.Not.Null);
			Assert.That(scope.ServiceProvider.GetRequiredService<IAuthenticationService>(), Is.Not.Null);
			Assert.That(scope.ServiceProvider.GetRequiredService<IOnboardingService>(), Is.Not.Null);
		});
	}

	private sealed class FakeTokenStorage : ILoginTokenStorage
	{
		public Task<string?> GetTokenAsync(CancellationToken cancellationToken = default) =>
			Task.FromResult<string?>(null);

		public Task StoreTokenAsync(string token, CancellationToken cancellationToken = default) =>
			Task.CompletedTask;

		public Task DeleteTokenAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

		public event EventHandler<TokenChangedEventArgs>? TokenChanged
		{
			add { }
			remove { }
		}
	}
}
