using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PubNet.Models;

public class Author
{
    [JsonIgnore]
    [Key]
    public int Id { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string Name { get; set; }

    [JsonIgnore]
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [JsonIgnore]
    public string PasswordHash { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Website { get; set; }

    public bool Inactive { get; set; }

    public DateTimeOffset RegisteredAtUtc { get; set; }

    [JsonIgnore]
    [InverseProperty(nameof(AuthorToken.Owner))]
    public ICollection<AuthorToken> Tokens { get; set; } = new List<AuthorToken>();

    [JsonIgnore]
    [InverseProperty(nameof(Package.Author))]
    public ICollection<Package> Packages { get; set; } = new List<Package>();
}