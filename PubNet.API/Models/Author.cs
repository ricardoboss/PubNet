using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace PubNet.API.Models;

[Index("Email", IsUnique = true)]
public class Author
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
    [ForeignKey(nameof(AuthorToken.Id))]
    public List<AuthorToken> Tokens { get; set; } = new();

    [JsonIgnore]
    [ForeignKey(nameof(Package.Id))]
    public List<Package> Packages { get; set; } = new();
}