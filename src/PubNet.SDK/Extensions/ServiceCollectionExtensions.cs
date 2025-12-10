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
			services.AddHttpClient<IRequestAdapter, HttpClientRequestAdapter>((sp, c) =>
			{
				c.DefaultRequestHeaders.UserAgent.Clear();
				c.DefaultRequestHeaders.UserAgent.Add(new("PubNet.SDK",
					typeof(PubNetApiClient).Assembly.GetName().Version!.ToString()));

				configureClient(sp, c);
			});
			services.TryAddTransient<IAuthenticationProvider, LoginTokenAuthenticationProvider>();
			services.TryAddScoped<PubNetApiClient>();
			services.TryAddScoped<IPackagesService, ApiPackagesService>();
			services.TryAddScoped<IAnalysisService, ApiAnalysisService>();

			return services;
		}
	}
}
