using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PubNet.BlobStorage.Abstractions;

namespace PubNet.BlobStorage.LocalFileBlobStorage;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddLocalFileStorage(this IServiceCollection serviceCollection, ServiceLifetime lifetime = ServiceLifetime.Singleton)
	{
		serviceCollection.TryAdd(new ServiceDescriptor(typeof(IBlobStorage), typeof(LocalFileBlobStorage), lifetime));

		return serviceCollection;
	}
}
