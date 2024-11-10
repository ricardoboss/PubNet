using PubNet.API.DTO.Packages.Nuget.Spec;
using PubNet.Database.Entities.Nuget;

namespace PubNet.API.DTO.Packages.Nuget;

public class NugetPackageVersionDto : PackageVersionDto
{
	public required string PackageId { get; init; }

	public string? Title { get; init; }

	public string? Description { get; init; }

	public string? Authors { get; init; }

	public string? IconUrl { get; init; }

	public string? ProjectUrl { get; init; }

	public string? Copyright { get; init; }

	public string? Tags { get; init; }

	public string? ReadmeFile { get; init; }

	public string? IconFile { get; init; }

	public NugetRepositoryMetadataDto? RepositoryMetadata { get; init; }

	public List<NugetPackageDependencyGroupDto>? DependencyGroups { get; init; }

	public static NugetPackageVersionDto MapFrom(NugetPackageVersion version)
	{
		return new NugetPackageVersionDto
		{
			PackageId = version.NuspecId!,
			Version = version.Version,
			PublishedAt = version.PublishedAt,
			Title = version.Title,
			Description = version.Description,
			Authors = version.Authors,
			IconUrl = version.IconUrl,
			ProjectUrl = version.ProjectUrl,
			Copyright = version.Copyright,
			Tags = version.Tags,
			ReadmeFile = version.ReadmeFile,
			IconFile = version.IconFile,
			RepositoryMetadata = version.RepositoryMetadata is null
				? null
				: NugetRepositoryMetadataDto.MapFrom(version.RepositoryMetadata),
			DependencyGroups = version.DependencyGroups?.Select(NugetPackageDependencyGroupDto.MapFrom).ToList(),
		};
	}
}
