using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using PubNet.DocsStorage.Abstractions;

namespace PubNet.DocsStorage.LocalFileDocsStorage;

public class LocalFileDocsStorage(IConfiguration configuration) : IDocsStorage
{
	private string RootPath => configuration["DocsStorage:RootPath"] ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PubNet.DocsStorage.LocalFileDocsStorage");

	/// <inheritdoc />
	public async Task StoreDocsAsync(string author, string name, string version, IFileProvider fileProvider, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var docsPath = Path.Combine(RootPath, author, name, version);

		if (Directory.Exists(docsPath))
			Directory.Delete(docsPath, true);

		Directory.CreateDirectory(docsPath);

		var fileProviderRoot = Path.GetDirectoryName(fileProvider.GetDirectoryContents("").First().PhysicalPath!)!;

		await CopyContentsRecursivelyAsync(new(fileProviderRoot), new(docsPath), cancellationToken);
	}

	private async Task CopyContentsRecursivelyAsync(DirectoryInfo source, DirectoryInfo destination, CancellationToken cancellationToken = default)
	{
		foreach (var sourceSubDirectory in source.GetDirectories())
		{
			var destinationSubDirectory = destination.CreateSubdirectory(sourceSubDirectory.Name);

			await CopyContentsRecursivelyAsync(sourceSubDirectory, destinationSubDirectory, cancellationToken);
		}

		foreach (var sourceFile in source.GetFiles())
		{
			var destinationFile = new FileInfo(Path.Combine(destination.FullName, sourceFile.Name));

			await using var sourceFileStream = sourceFile.OpenRead();
			await using var destinationFileStream = destinationFile.OpenWrite();

			await sourceFileStream.CopyToAsync(destinationFileStream, cancellationToken);
		}
	}

	/// <inheritdoc />
	public IFileProvider GetDocsFileProvider(string author, string name, string version, CancellationToken cancellationToken = default)
	{
		var docsPath = Path.Combine(RootPath, author, name, version);

		return new PhysicalFileProvider(docsPath);
	}
}

public class DocsStorageException(string s) : Exception(s);
