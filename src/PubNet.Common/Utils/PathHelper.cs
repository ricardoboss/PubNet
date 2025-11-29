namespace PubNet.Common.Utils;

public static class PathHelper
{
	public static Task<string?> GetCaseInsensitivePath(string workingDirectory, string searchFilename, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var lowercaseSearchFilename = searchFilename.ToLowerInvariant();
		return Task.FromResult(Directory.EnumerateFiles(workingDirectory).FirstOrDefault(
			filename =>
			{
				var lowercaseFilename = Path.GetFileName(filename).ToLowerInvariant();

				return lowercaseFilename.SequenceEqual(lowercaseSearchFilename);
			}));
	}
}
