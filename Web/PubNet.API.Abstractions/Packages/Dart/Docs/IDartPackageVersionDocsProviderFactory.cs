namespace PubNet.API.Abstractions.Packages.Dart.Docs;

public interface IDartPackageVersionDocsProviderFactory
{
	Task<IDartPackageVersionDocsProvider> CreateAsync(string packageName, string packageVersion, CancellationToken cancellationToken = default);
}
