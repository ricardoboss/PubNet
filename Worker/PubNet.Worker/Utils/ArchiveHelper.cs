using SharpCompress.Readers;

namespace PubNet.Worker.Utils;

[Obsolete("Use IArchiveReader instead")]
public static class ArchiveHelper
{
	public static void UnpackInto(Stream archiveStream, string destinationDirectory)
	{
		Directory.CreateDirectory(destinationDirectory);

		using var reader = ReaderFactory.Open(archiveStream);

		while (reader.MoveToNextEntry())
			if (!reader.Entry.IsDirectory)
				reader.WriteEntryToDirectory(destinationDirectory, new()
				{
					ExtractFullPath = true,
					Overwrite = true,
				});
	}
}
