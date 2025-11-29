namespace PubNet.Common.Interfaces;

public interface IFileContainer : IFilesystemEntry
{
	IFileContainer? Parent { get; }

	IEnumerable<IFilesystemEntry> GetEntries();
}
