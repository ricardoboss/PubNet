namespace PubNet.API.Abstractions.Archives;

/// <summary>
/// Represents an entry in an archive.
/// </summary>
public interface IArchiveEntry
{
	/// <summary>
	/// The name of the entry.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Whether the entry is a directory.
	/// </summary>
	bool IsDirectory { get; }

	/// <summary>
	/// Opens the entry for reading.
	/// </summary>
	/// <returns>A <see cref="Stream"/> that can be used to read the entry.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the entry is a directory.</exception>
	Stream OpenRead();
}
