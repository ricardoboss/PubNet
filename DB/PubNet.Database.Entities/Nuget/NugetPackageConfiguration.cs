using System.Linq.Expressions;
using PubNet.Database.Entities.Packages;

namespace PubNet.Database.Entities.Nuget;

public class NugetPackageConfiguration : BasePackageConfiguration<NugetPackage, NugetPackageVersion>
{
	protected override Expression<Func<Author, IEnumerable<NugetPackage>?>> NavigateFromAuthorToPackages()
	{
		return a => a.NugetPackages;
	}
}
