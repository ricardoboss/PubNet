using PubNet.Client.Abstractions;

namespace PubNet.Client.Extensions;

public static class PackagesServiceExtensions
{
	public static IPackagesQueryBuilder<TPackage, TPackageList> GetPackages<TPackage, TPackageList>(this IPackagesService<TPackage, TPackageList> service)
	{
		return new DefaultPackagesQueryBuilder<TPackage, TPackageList>(service);
	}
}
