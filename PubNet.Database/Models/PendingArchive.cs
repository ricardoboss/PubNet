namespace PubNet.Database.Models;

public class PendingArchive
{
	public int Id { get; set; }

	public Guid Uuid { get; set; }

	public string ArchivePath { get; set; } = string.Empty;

	public string UnpackedArchivePath => ArchivePath[..^".tar.gz".Length];

	public int UploaderId { get; set; }

	public Author? Uploader { get; set; }

	public DateTimeOffset UploadedAtUtc { get; set; }
}
