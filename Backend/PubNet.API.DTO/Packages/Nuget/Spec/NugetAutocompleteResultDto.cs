namespace PubNet.API.DTO.Packages.Nuget.Spec;

public class NugetAutocompleteResultDto
{
	public long TotalHits { get; init; }

	public IEnumerable<string> Data { get; init; } = new List<string>();
}
