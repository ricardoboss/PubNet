using PubNet.Models;

namespace PubNet.API.Models;

public class PackageVersionAnalysis
{
    public int Id { get; set; }

    public PackageVersion Version { get; set; }

    public bool? Formatted { get; set; }

    public string? DocumentationLink { get; set; }
}
