using PubNet.API.Abstractions.Archives;
using SharpCompress.Readers;
using SharpCompress.Readers.Tar;

namespace PubNet.API.Services.Archives;

/// <summary>
/// Extracts the archive to a temporary directory and then iterates over the entries in that directory.
/// </summary>
public class TempDirExtractingArchiveReader : IArchiveReader
{
	/// <inheritdoc />
	public IEnumerable<IArchiveEntry> EnumerateEntries(Stream archive, bool leaveStreamOpen = true)
	{
		var tempDir = Path.Combine(Path.GetTempPath(), "PubNet", "ArchiveReader", Guid.NewGuid().ToString());
		if (Directory.Exists(tempDir))
			Directory.Delete(tempDir, true);

		Directory.CreateDirectory(tempDir);

		if (!archive.CanSeek)
			throw new InvalidOperationException("Stream must be seekable");

		archive.Seek(0, SeekOrigin.Begin);

		using (var reader = TarReader.Open(archive, new()
		{
			LeaveStreamOpen = leaveStreamOpen,
		}))
		{
			while (reader.MoveToNextEntry())
				if (!reader.Entry.IsDirectory)
					reader.WriteEntryToDirectory(tempDir, new()
					{
						ExtractFullPath = true,
						Overwrite = true,
					});
		}

		foreach (var entry in Directory.EnumerateFileSystemEntries(tempDir))
		{
			yield return new FileArchiveEntry(entry)
			{
				Name = Path.GetFileName(entry),
				IsDirectory = Directory.Exists(entry),
			};
		}

		Directory.Delete(tempDir, true);
	}
}

file class FileArchiveEntry(string filePath) : IArchiveEntry
{
	public required string Name { get; init; }

	public required bool IsDirectory { get; init; }

	public Stream OpenRead() => File.OpenRead(filePath);
}
