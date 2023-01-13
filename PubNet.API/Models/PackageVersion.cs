using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PubNet.API.Models;

public class PackageVersion
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Version { get; set; }

    public bool Retracted { get; set; }

    [JsonPropertyName("archive_url")]
    public string ArchiveUrl { get; set; }

    [JsonPropertyName("archive_sha256")]
    public string ArchiveSha256 { get; set; }

    [JsonPropertyName("published")]
    public DateTimeOffset PublishedAtUtc { get; set; }

    [JsonPropertyName("published_millis")]
    public long PublishedAtMillis => PublishedAtUtc.ToUnixTimeMilliseconds();

    [JsonIgnore]
    [Column(TypeName = "json")]
    public string Pubspec { get; set; }

    [JsonPropertyName("pubspec")]
    public PubSpec? PubspecJson => JsonSerializer.Deserialize<PubSpec>(Pubspec, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    }) ?? null;
}