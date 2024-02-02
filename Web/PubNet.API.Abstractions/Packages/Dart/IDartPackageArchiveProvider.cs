namespace PubNet.API.Abstractions.Packages.Dart;

public interface IDartPackageArchiveProvider
{
	Task<Stream?> GetArchiveAsync(string name, string version, CancellationToken cancellationToken = default);
}
