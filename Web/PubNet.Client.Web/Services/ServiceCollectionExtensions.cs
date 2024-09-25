using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PubNet.Client.Abstractions;

namespace PubNet.Client.Web.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddBrowserLoginTokenStorage(this IServiceCollection services)
	{
		services.AddBlazoredLocalStorageAsSingleton();
		services.TryAddSingleton<ILoginTokenStorage, BrowserLoginTokenStorage>();

		return services;
	}
}
