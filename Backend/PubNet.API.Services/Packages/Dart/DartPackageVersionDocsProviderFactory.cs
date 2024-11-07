using PubNet.API.Abstractions.Packages.Dart.Docs;
using PubNet.DocsStorage.Abstractions;

namespace PubNet.API.Services.Packages.Dart;

public class DartPackageVersionDocsProviderFactory : IDartPackageVersionDocsProviderFactory
{
	public async Task<IDocsFileProvider> CreateAsync(string packageName, string packageVersion, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
