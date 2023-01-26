using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace PubNet.Models;

public class Author : IdentityUser<int>
{
    [JsonIgnore]
    [Key]
    public new int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

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