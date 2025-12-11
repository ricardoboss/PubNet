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
	extension(IServiceCollection services)
	{
		public IServiceCollection AddPubNetApi(Action<IServiceProvider, HttpClient> configureClient)
		{
			services.TryAddTransient<IAuthenticationProvider, LoginTokenAuthenticationProvider>();
			services.AddHttpClient();
			services.TryAddScoped<IRequestAdapter>(sp =>
			{
				var authenticationProvider = sp.GetRequiredService<IAuthenticationProvider>();
				var client = sp.GetRequiredService<HttpClient>();

				client.DefaultRequestHeaders.UserAgent.Clear();
				client.DefaultRequestHeaders.UserAgent.Add(new("PubNet.SDK",
					typeof(PubNetApiClient).Assembly.GetName().Version!.ToString()));
				configureClient(sp, client);

				var adapter = new HttpClientRequestAdapter(
					authenticationProvider: authenticationProvider,
					httpClient: client
				);

				return adapter;
			});
			services.TryAddScoped<PubNetApiClient>();
			services.TryAddScoped<IPackagesService, ApiPackagesService>();
			services.TryAddScoped<IAnalysisService, ApiAnalysisService>();
			services.TryAddScoped<IAuthenticationService, ApiAuthenticationService>();
			services.TryAddScoped<IAuthorService, ApiAuthorService>();

			return services;
		}
	}
}
