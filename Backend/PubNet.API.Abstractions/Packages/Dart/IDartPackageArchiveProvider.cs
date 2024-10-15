namespace PubNet.API.Abstractions.Packages.Dart;

public interface IDartPackageArchiveProvider
{
	Task<Stream?> GetArchiveContentAsync(string name, string version, CancellationToken cancellationToken = default);

	(Uri url, string sha256) GetArchiveUriAndHash(string name, string version);
}
