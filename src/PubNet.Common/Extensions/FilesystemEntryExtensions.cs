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
			var segments = new Queue<string>(path.TrimEnd('/', '\\').Split('/').SelectMany(s => s.Split('\\')));

			var currentContainer = container;
			while (segments.TryDequeue(out var segment))
			{
				if (segment.Length == 0)
					continue;

				if (currentContainer.GetChildEntry(segment) is not { } child)
					return null;

				if (segments.Count == 0)
					return child;

				if (child is IFileContainer subContainer)
				{
					currentContainer = subContainer;
					continue;
				}

				// cannot navigate into files and we already handled file containers
				if (segments.Count != 0)
					return null;
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
