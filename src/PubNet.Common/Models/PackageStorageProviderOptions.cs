using Microsoft.Extensions.DependencyInjection;

namespace PubNet.Common.Models;

public class PackageStorageProviderOptions
{
	public const string ConfigKey = "PackageStorage";

	public string? Path { get; set; }
}

public static class PackageStorageProviderOptionsServiceCollectionExtensions
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddPackageStorageProviderOptions()
		{
			services.AddOptions<PackageStorageProviderOptions>()
				.BindConfiguration(PackageStorageProviderOptions.ConfigKey);

			return services;
		}
	}
}
