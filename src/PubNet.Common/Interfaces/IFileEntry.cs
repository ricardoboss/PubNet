namespace PubNet.Common.Interfaces;

public interface IFileEntry : IFilesystemEntry
{
	Stream OpenRead();
}
