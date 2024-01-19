using Microsoft.Extensions.FileProviders;

namespace PubNet.DocsStorage.Abstractions;

public interface IDocsFileProvider : IFileProvider
{
	string PackageName { get; }

	string PackageVersion { get; }

	IFileInfo GetNotFoundFile();
}
