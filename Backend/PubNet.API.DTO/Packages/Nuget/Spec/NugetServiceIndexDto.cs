namespace PubNet.API.DTO.Packages.Nuget.Spec;

public class NugetServiceIndexDto
{
	public string Version { get; init; } = "3.0.0";

	public IEnumerable<NugetServiceIndexResourceDto> Resources { get; init; } = [];
}
