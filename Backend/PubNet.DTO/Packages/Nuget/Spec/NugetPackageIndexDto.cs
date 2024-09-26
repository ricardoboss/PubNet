namespace PubNet.API.DTO.Packages.Nuget.Spec;

public class NugetPackageIndexDto
{
	public IEnumerable<string> Versions { get; init; } = new List<string>();
}
