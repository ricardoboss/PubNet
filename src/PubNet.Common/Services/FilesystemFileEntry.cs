using JetBrains.Annotations;
using PubNet.Common.Interfaces;

namespace PubNet.Common.Services;

public class FilesystemFileEntry(FileInfo info) : IFileEntry
{
	public static FilesystemFileEntry FromPath(string path)
	{
		var info = new FileInfo(path);

		return new(info);
	}

	public string Name => Info.Name;

	public FileInfo Info => info;

	[MustDisposeResource]
	public Stream OpenRead() => Info.OpenRead();
}
