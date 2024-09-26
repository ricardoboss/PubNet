using PubNet.DocsStorage.Abstractions;

namespace PubNet.API.Abstractions.Packages.Dart.Docs;

public interface IDartPackageVersionDocsProviderFactory
{
	Task<IDocsFileProvider> CreateAsync(string packageName, string packageVersion, CancellationToken cancellationToken = default);
}
