using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace PubNet.API.Models;

[Index("Value", IsUnique = true)]
public class AuthorToken
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Value { get; set; }

    public DateTimeOffset ExpiresAtUtc { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(Author.Id))]
    public int OwnerId { get; set; }

    [JsonIgnore]
    public Author? Owner { get; set; }

    public bool IsValid()
    {
        return ExpiresAtUtc > DateTimeOffset.UtcNow;
    }
}