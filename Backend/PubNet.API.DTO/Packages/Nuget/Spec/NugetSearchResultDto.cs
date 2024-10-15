namespace PubNet.API.DTO.Packages.Nuget.Spec;

public class NugetSearchResultDto
{
	public static NugetSearchResultDto MapFrom(NugetPackageListDto list)
	{
		return new()
		{
			TotalHits = list.TotalHits,
			Data = list.Packages.Select(NugetSearchResultHitDto.MapFrom),
		};
	}

	/// <summary>
	/// The total number of matches, disregarding <c>skip</c> and <c>take</c>.
	/// </summary>
	public required long TotalHits { get; init; }

	/// <summary>
	/// The search results matched by the request.
	/// </summary>
	public required IEnumerable<NugetSearchResultHitDto> Data { get; init; } = [];
}
