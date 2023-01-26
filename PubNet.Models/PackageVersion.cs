using System.Text.Json.Serialization;

namespace PubNet.Models;

public class PackageVersion
{
    [JsonIgnore]
    public int Id { get; set; }

    public string PackageName { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public bool Retracted { get; set; }

    [JsonPropertyName("archive_url")]
    public string ArchiveUrl { get; set; } = string.Empty;

    [JsonPropertyName("archive_sha256")]
    public string ArchiveSha256 { get; set; } = string.Empty;

    [JsonPropertyName("published")]
    public DateTimeOffset PublishedAtUtc { get; set; }

    [JsonPropertyName("published_millis")]
    public long PublishedAtMillis => PublishedAtUtc.ToUnixTimeMilliseconds();

    [JsonPropertyName("pubspec")]
    public PubSpec? PubSpec { set; get; }
}