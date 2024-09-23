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
	/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
	/// <returns>An <see cref="IAsyncEnumerable{T}"/> that can be used to iterate over the entries in the archive.</returns>
	IAsyncEnumerable<IArchiveEntry> ReadAsync(Stream archive, CancellationToken cancellationToken = default);
}
