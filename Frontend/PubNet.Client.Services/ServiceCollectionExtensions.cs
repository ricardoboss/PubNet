using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Http.HttpClientLibrary;
using PubNet.Client.Abstractions;
using PubNet.Client.ApiClient.Generated;

namespace PubNet.Client.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddPubNetApiClient(this IServiceCollection services, Action<HttpClient>? configureHttpClient = null)
	{
		services.TryAddTransient<IParseNodeFactory>(_ => ParseNodeFactoryRegistry.DefaultInstance);
		services.TryAddTransient<ISerializationWriterFactory>(_ => SerializationWriterFactoryRegistry.DefaultInstance);
		services.TryAddTransient<IAuthenticationProvider, LoginTokenAuthenticationProvider>();
		services.TryAddSingleton<HttpClient>(_ =>
		{
			var httpClient = new HttpClient();

			configureHttpClient?.Invoke(httpClient);

			return httpClient;
		});
		services.TryAddTransient<IRequestAdapter, HttpClientRequestAdapter>();
		services.TryAddSingleton<PubNetApiClient>();

		services.TryAddSingleton<ILoginService, ApiAuthService>();
		services.TryAddSingleton<IAuthenticationService, ApiAuthService>();
		services.TryAddSingleton<IRegisterService, ApiRegisterService>();
		services.TryAddSingleton<IPersonalAccessTokenService, ApiPersonalAccessTokenService>();
		services.TryAddSingleton<IDartPackagesService, ApiDartPackagesService>();
		services.TryAddSingleton<IAuthorsService, ApiAuthorsService>();
		services.TryAddSingleton<IOnboardingService, ApiOnboardingService>();
		services.TryAddSingleton<IAdminService, ApiAdminService>();

		return services;
	}
}
