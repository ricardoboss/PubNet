using PubNet.Storage.Utils.Abstractions.Archives;
using SharpCompress.Readers;
using SharpCompress.Readers.Tar;

namespace PubNet.Storage.Utils.Archives;

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

		ReadIntoDirectory(archive, tempDir, leaveStreamOpen);

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

	public void ReadIntoDirectory(Stream archive, string destinationDirectory, bool leaveStreamOpen = true)
	{
		Directory.CreateDirectory(destinationDirectory);

		if (!archive.CanSeek)
			throw new InvalidOperationException("Stream must be seekable");

		archive.Seek(0, SeekOrigin.Begin);

		using var reader = TarReader.Open(archive, new()
		{
			LeaveStreamOpen = leaveStreamOpen,
		});

		while (reader.MoveToNextEntry())
		{
			if (!reader.Entry.IsDirectory)
			{
				reader.WriteEntryToDirectory(destinationDirectory, new()
				{
					ExtractFullPath = true,
					Overwrite = true,
				});
			}
		}
	}
}

file class FileArchiveEntry(string filePath) : IArchiveEntry
{
	public required string Name { get; init; }

	public required bool IsDirectory { get; init; }

	public Stream OpenRead() => File.OpenRead(filePath);
}
