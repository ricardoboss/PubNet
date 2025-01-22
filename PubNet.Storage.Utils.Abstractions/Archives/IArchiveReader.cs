namespace PubNet.Storage.Utils.Abstractions.Archives;

/// <summary>
/// Enables iterating over the entries in an archive.
/// </summary>
public interface IArchiveReader
{
	/// <summary>
	/// Reads the entries in the given archive recursively.
	/// </summary>
	/// <param name="archive">The archive stream to read.</param>
	/// <param name="leaveStreamOpen">Whether to leave the stream open after reading the entries.</param>
	/// <returns>An <see cref="IEnumerable{T}"/> that can be used to iterate over the entries in the archive.</returns>
	IEnumerable<IArchiveEntry> EnumerateEntries(Stream archive, bool leaveStreamOpen = true);

	/// <summary>
	/// Unpacks the archive into the given directory.
	/// </summary>
	/// <param name="archive">The archive stream to read.</param>
	/// <param name="destinationDirectory">The directory to unpack the archive into.</param>
	/// <param name="leaveStreamOpen">Whether to leave the stream open after reading the entries.</param>
	void ReadIntoDirectory(Stream archive, string destinationDirectory, bool leaveStreamOpen = true);
}
