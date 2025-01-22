namespace PubNet.PackageStorage.Abstractions;

public class PackageStorageOptions
{
	public TimeSpan? PendingMaxAge { get; set; }

	public string? Path { get; set; }

	public long? MaxFileSize { get; set; }
}
