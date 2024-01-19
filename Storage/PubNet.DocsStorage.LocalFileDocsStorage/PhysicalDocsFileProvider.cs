using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using PubNet.DocsStorage.Abstractions;

namespace PubNet.DocsStorage.LocalFileDocsStorage;

public class PhysicalDocsFileProvider : PhysicalFileProvider, IDocsFileProvider
{
	private readonly string notFoundFileName;

	/// <inheritdoc />
	public PhysicalDocsFileProvider(string packageName, string packageVersion, string root, string notFoundFileName) : base(root)
	{
		PackageName = packageName;
		PackageVersion = packageVersion;
		this.notFoundFileName = notFoundFileName;
	}

	/// <inheritdoc />
	public PhysicalDocsFileProvider(string packageName, string packageVersion, string root, string notFoundFileName, ExclusionFilters filters) : base(root, filters)
	{
		PackageName = packageName;
		PackageVersion = packageVersion;
		this.notFoundFileName = notFoundFileName;
	}

	/// <inheritdoc />
	public string PackageName { get; }

	/// <inheritdoc />
	public string PackageVersion { get; }

	/// <inheritdoc />
	public IFileInfo GetNotFoundFile() => GetFileInfo(notFoundFileName);
}
