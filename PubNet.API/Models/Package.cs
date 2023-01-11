using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace PubNet.API.Models;

[Index("Name", IsUnique = true)]
public class Package
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsDiscontinued { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ReplacedBy { get; set; }

    public PackageVersion Latest { get; set; }

    [ForeignKey(nameof(PackageVersion.Id))]
    public List<PackageVersion> Versions { get; set; } = new();

    [JsonIgnore]
    [ForeignKey(nameof(PubNet.API.Models.Author.Id))]
    public int AuthorId { get; set; }

    [JsonIgnore]
    public Author? Author { get; set; }
}