using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace PubNet.Models;

public class Author : IdentityUser<int>
{
    [Key]
    public new int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public override string UserName { get; set; } = string.Empty;

    public string? Website { get; set; }

    public bool Inactive { get; set; }

    public DateTimeOffset RegisteredAtUtc { get; set; }

    public ICollection<Package> Packages { get; set; } = new List<Package>();
}
