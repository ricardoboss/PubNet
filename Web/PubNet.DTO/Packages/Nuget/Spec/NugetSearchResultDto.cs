namespace PubNet.API.DTO.Packages.Nuget.Spec;

public class NugetSearchResultDto
{
	/// <summary>
	/// The total number of matches, disregarding <c>skip</c> and <c>take</c>.
	/// </summary>
	public required long TotalHits { get; init; }

	/// <summary>
	/// The search results matched by the request.
	/// </summary>
	public required IEnumerable<NugetSearchResultHitDto> Data { get; init; } = [];
}
