using Microsoft.Extensions.FileProviders;

namespace PubNet.API.Abstractions.Packages.Dart.Docs;

public interface IDartPackageVersionDocsProvider : IFileProvider
{
	string PackageName { get; }

	string PackageVersion { get; }

	IFileInfo GetNotFoundFile();
}
