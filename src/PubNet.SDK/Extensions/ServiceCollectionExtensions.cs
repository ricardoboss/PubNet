using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Generated;
using PubNet.SDK.Services;

namespace PubNet.SDK.Extensions;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Name of the <see cref="IHttpClientFactory"/> client the SDK sends its requests with.
	/// </summary>
	/// <remarks>
	/// The SDK deliberately does not use the default (unnamed) client: that one belongs to the consuming
	/// application, and configuring it here would mean reaching into a client someone else also uses.
	/// </remarks>
	public const string HttpClientName = "PubNet.SDK";

	extension(IServiceCollection services)
	{
		public IPubNetApiServiceBuilder AddPubNetApiServices<TTokenStorage>(
			Action<IServiceProvider, HttpClient> configureClient
		) where TTokenStorage : class, ILoginTokenStorage
		{
			services.AddHttpClient(HttpClientName, (sp, client) =>
			{
				client.DefaultRequestHeaders.UserAgent.Clear();
				client.DefaultRequestHeaders.UserAgent.Add(new("PubNet.SDK",
					typeof(PubNetApiClient).Assembly.GetName().Version!.ToString()));

				configureClient(sp, client);
			});

			services.TryAddScoped<IRequestAdapter>(sp =>
			{
				var authenticationProvider = sp.GetRequiredService<IAuthenticationProvider>();
				var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientName);

				var adapter = new HttpClientRequestAdapter(
					authenticationProvider: authenticationProvider,
					httpClient: client
				);

				return adapter;
			});

			services.TryAddScoped<ILoginTokenStorage, TTokenStorage>();
			services.TryAddTransient<IAuthenticationProvider, LoginTokenAuthenticationProvider>();
			services.TryAddScoped<PubNetApiClient>();

			services.TryAddScoped<IPackagesService, ApiPackagesService>();
			services.TryAddScoped<IAnalysisService, ApiAnalysisService>();
			services.TryAddScoped<IAuthenticationService, ApiAuthenticationService>();
			services.TryAddScoped<IAuthorService, ApiAuthorService>();
			services.TryAddScoped<IOnboardingService, ApiOnboardingService>();

			return new DefaultPubNetApiServiceBuilder(services);
		}

		public IPubNetApiServiceBuilder AddPubNetApiServices<TTokenStorage>(Uri baseAddress)
			where TTokenStorage : class, ILoginTokenStorage
		{
			return services.AddPubNetApiServices<TTokenStorage>((_, c) => c.BaseAddress = baseAddress);
		}
	}
}
