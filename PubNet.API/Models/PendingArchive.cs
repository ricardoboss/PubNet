namespace PubNet.API.Models;

public class PendingArchive
{
    public int Id { get; set; }

    public string ArchivePath { get; set; }

    public int UploaderId { get; set; }

    public AuthorToken Uploader { get; set; }

    public DateTimeOffset UploadedAtUtc { get; set; }
}