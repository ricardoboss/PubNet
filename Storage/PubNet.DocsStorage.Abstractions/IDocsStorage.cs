using Microsoft.Extensions.FileProviders;

namespace PubNet.DocsStorage.Abstractions;

public interface IDocsStorage
{
	public Task StoreDocsAsync(string author, string name, string version, IFileProvider fileProvider, CancellationToken cancellationToken = default);

	public IFileProvider GetDocsFileProvider(string author, string name, string version, CancellationToken cancellationToken = default);
}
