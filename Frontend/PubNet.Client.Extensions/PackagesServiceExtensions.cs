using PubNet.Client.Abstractions;

namespace PubNet.Client.Extensions;

public static class PackagesServiceExtensions
{
	public static IPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList> GetPackages<TPackage, TPackageVersion, TPackageList>(
		this IPackagesService<TPackage, TPackageVersion, TPackageList> service)
	{
		return new DefaultPackagesQueryBuilder<TPackage, TPackageVersion, TPackageList>(service);
	}
}
