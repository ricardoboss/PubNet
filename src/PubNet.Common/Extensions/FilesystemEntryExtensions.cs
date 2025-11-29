using System.Collections;
using PubNet.Common.Interfaces;

namespace PubNet.Common.Extensions;

public static class FilesystemEntryExtensions
{
	extension(IFileContainer container)
	{
		public async Task CopyToAsync(DirectoryInfo destination, CancellationToken cancellationToken = default)
		{
			if (!destination.Exists)
				destination.Create();

			foreach (var entry in container.GetEntries())
			{
				switch (entry)
				{
					case IFileEntry file:
						var destinationFilePath = Path.Combine(destination.FullName, file.Name);
						var destinationFileInfo = new FileInfo(destinationFilePath);

						await file.CopyToAsync(destinationFileInfo, cancellationToken);
						break;
					case IFileContainer subContainer:
						var subDirPath = Path.Combine(destination.FullName, subContainer.Name);
						var subDirInfo = new DirectoryInfo(subDirPath);

						await subContainer.CopyToAsync(subDirInfo, cancellationToken);
						break;
					default:
						throw new NotImplementedException("Unknown filesystem entry type: " + entry.GetType());
				}
			}
		}

		public IFilesystemEntry? GetChildEntry(string name)
		{
			return container
				.GetEntries()
				.FirstOrDefault(child => string.Equals(child.Name, name, StringComparison.InvariantCultureIgnoreCase));
		}

		public IFilesystemEntry? GetRelativeEntry(string path)
		{
			var segments = new Queue<string>(path.Split(Path.DirectorySeparatorChar));

			var currentContainer = container;
			while (segments.TryDequeue(out var segment))
			{
				if (currentContainer.GetChildEntry(segment) is not { } child)
					return null;

				switch (child)
				{
					case IFileEntry file when segments.Count == 0:
						return file;
					case IFileContainer subContainer when segments.Count > 0:
						currentContainer = subContainer;
						continue;
					default:
						throw new NotImplementedException("Unknown filesystem entry type: " + child.GetType());
				}
			}

			return null;
		}
	}

	extension(IFileEntry file)
	{
		public async Task CopyToAsync(FileInfo destination, CancellationToken ct)
		{
			await using var destinationStream = destination.OpenWrite();
			await using var sourceStream = file.OpenRead();

			await sourceStream.CopyToAsync(destinationStream, ct);
		}
	}
}
