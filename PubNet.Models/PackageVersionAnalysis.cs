namespace PubNet.Models;

public class PackageVersionAnalysis
{
    public int Id { get; set; }

    public int VersionId { get; set; }

    public PackageVersion Version { get; set; }

    public bool? Formatted { get; set; }

    public string? DocumentationLink { get; set; }

    public DateTimeOffset? CompletedAtUtc { get; set; }
}
