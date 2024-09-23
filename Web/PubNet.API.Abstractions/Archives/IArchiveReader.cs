namespace PubNet.API.Abstractions.Archives;

/// <summary>
/// Enables iterating over the entries in an archive.
/// </summary>
public interface IArchiveReader
{
	/// <summary>
	/// Reads the entries in the given archive recursively.
	/// </summary>
	/// <param name="archive">The archive stream to read.</param>
	/// <returns>An <see cref="IEnumerable{T}"/> that can be used to iterate over the entries in the archive.</returns>
	IEnumerable<IArchiveEntry> EnumerateEntries(Stream archive);
}
