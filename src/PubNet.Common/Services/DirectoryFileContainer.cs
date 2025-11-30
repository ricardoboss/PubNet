using PubNet.Common.Interfaces;

namespace PubNet.Common.Services;

public class DirectoryFileContainer(DirectoryInfo info) : IFileContainer
{
	public static DirectoryFileContainer FromPath(string path)
	{
		var info = new DirectoryInfo(path);

		return new(info);
	}

	public IFileContainer? Parent => Info.Parent is { } parentInfo ? new DirectoryFileContainer(parentInfo) : null;

	public string Name => Info.Name;

	public DirectoryInfo Info => info;

	public IEnumerable<IFilesystemEntry> GetEntries()
	{
		foreach (var systemInfo in Info.EnumerateFileSystemInfos())
		{
			yield return systemInfo switch
			{
				FileInfo fileInfo => new FilesystemFileEntry(fileInfo),
				DirectoryInfo dirInfo => new DirectoryFileContainer(dirInfo),
				_ => throw new NotImplementedException("Filesystem entry type " + systemInfo.GetType() +
					" not implemented"),
			};
		}
	}
}
