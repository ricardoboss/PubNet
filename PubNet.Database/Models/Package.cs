namespace PubNet.Database.Models;

public class Package
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsDiscontinued { get; set; }

    public string? ReplacedBy { get; set; }

    public PackageVersion? Latest { get; set; }

    public ICollection<PackageVersion> Versions { get; set; } = new List<PackageVersion>();

    public int? AuthorId { get; set; }

    public Author? Author { get; set; }
}