using PubNet.Models;

namespace PubNet.API.Models;

public class PendingArchive
{
    public int Id { get; set; }

    public Guid Uuid { get; set; }

    public string ArchivePath { get; set; }

    public string UnpackedArchivePath => ArchivePath[..^".tar.gz".Length];

    public int UploaderId { get; set; }

    public AuthorToken Uploader { get; set; }

    public DateTimeOffset UploadedAtUtc { get; set; }
}