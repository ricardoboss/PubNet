using SharpCompress.Readers;

namespace PubNet.Common.Utils;

public static class ArchiveHelper
{
	public static async Task UnpackIntoAsync(Stream archiveStream, string destinationDirectory, CancellationToken cancellationToken = default)
	{
		Directory.CreateDirectory(destinationDirectory);

		await using var reader = await ReaderFactory.OpenAsyncReader(archiveStream, cancellationToken: cancellationToken);

		await reader.WriteAllToDirectoryAsync(destinationDirectory, cancellationToken: cancellationToken);
	}
}
