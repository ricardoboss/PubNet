using PubNet.API.Abstractions.Archives;
using SharpCompress.Readers;

namespace PubNet.API.Services.Archives;

/// <summary>
/// Extracts the archive to a temporary directory and then iterates over the entries in that directory.
/// </summary>
public class TempDirExtractingArchiveReader : IArchiveReader
{
	/// <inheritdoc />
	public IEnumerable<IArchiveEntry> EnumerateEntries(Stream archive)
	{
		using var reader = ReaderFactory.Open(archive);

		var tempDir = Path.Combine(Path.GetTempPath(), "PubNet", "ArchiveReader", Guid.NewGuid().ToString());
		if (Directory.Exists(tempDir))
			Directory.Delete(tempDir, true);

		Directory.CreateDirectory(tempDir);

		reader.WriteAllToDirectory(tempDir, new()
		{
			ExtractFullPath = true,
			Overwrite = true,
			PreserveAttributes = true,
			PreserveFileTime = true,
		});

		foreach (var entry in Directory.EnumerateFileSystemEntries(tempDir))
		{
			yield return new SharpCompressArchiveEntry(entry)
			{
				Name = Path.GetFileName(entry),
				IsDirectory = Directory.Exists(entry),
			};
		}

		Directory.Delete(tempDir, true);
	}
}

file class SharpCompressArchiveEntry(string filePath) : IArchiveEntry
{
	public required string Name { get; init; }

	public required bool IsDirectory { get; init; }

	public Stream OpenRead() => File.OpenRead(filePath);
}
