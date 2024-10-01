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
	public static IServiceCollection AddPubNetApiClient(this IServiceCollection services, Action<HttpClient>? configureHttpClient = default)
	{
		services.TryAddTransient<IParseNodeFactory>(_ => ParseNodeFactoryRegistry.DefaultInstance);
		services.TryAddTransient<ISerializationWriterFactory>(_ => SerializationWriterFactoryRegistry.DefaultInstance);
		services.TryAddTransient<IAuthenticationProvider, LoginTokenAuthenticationProvider>();
		services.TryAddScoped<HttpClient>(_ =>
		{
			var httpClient = new HttpClient();

			configureHttpClient?.Invoke(httpClient);

			return httpClient;
		});
		services.TryAddTransient<IRequestAdapter, HttpClientRequestAdapter>();
		services.TryAddScoped<PubNetApiClient>();

		services.TryAddScoped<ILoginService, ApiLoginService>();
		services.TryAddScoped<IRegisterService, ApiRegisterService>();
		services.TryAddScoped<IPersonalAccessTokenService, ApiPersonalAccessTokenService>();

		return services;
	}
}
