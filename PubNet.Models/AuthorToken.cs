using System.Text.Json.Serialization;

namespace PubNet.Models;

public class AuthorToken
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Name { get; set; }

    public byte[] Value { get; set; }

    public DateTimeOffset ExpiresAtUtc { get; set; }

    [JsonIgnore]
    public int OwnerId { get; set; }

    [JsonIgnore]
    public Author Owner { get; set; }

    public bool IsValid()
    {
        return ExpiresAtUtc > DateTimeOffset.UtcNow;
    }
}