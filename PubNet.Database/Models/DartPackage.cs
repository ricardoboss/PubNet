namespace PubNet.Database.Models;

public class DartPackage
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public bool IsDiscontinued { get; set; }

	public string? ReplacedBy { get; set; }

	public DartPackageVersion? Latest { get; set; }

	public ICollection<DartPackageVersion> Versions { get; set; } = new List<DartPackageVersion>();

	public int? AuthorId { get; set; }

	public Author? Author { get; set; }
}
