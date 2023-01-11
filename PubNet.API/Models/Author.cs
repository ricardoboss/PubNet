using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace PubNet.API.Models;

[Index("Email", IsUnique = true)]
public class Author
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    [JsonIgnore]
    public string PasswordHash { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Website { get; set; }

    public bool Inactive { get; set; }

    public DateTimeOffset RegisteredAtUtc { get; set; }

    [JsonIgnore]
    [InverseProperty(nameof(AuthorToken.Owner))]
    public List<AuthorToken> Tokens { get; set; } = new();

    [JsonIgnore]
    [InverseProperty(nameof(Package.Author))]
    public List<Package> Packages { get; set; } = new();
}