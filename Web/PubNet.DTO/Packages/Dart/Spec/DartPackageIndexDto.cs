namespace PubNet.API.DTO.Packages.Dart.Spec;

public class DartPackageIndexDto
{
	public required string Name { get; init; }

	public bool? IsDiscontinued { get; init; }

	public string? ReplacedBy { get; init; }

	public required DartPackageVersionDto Latest { get; init; }

	public required List<DartPackageVersionDto> Versions { get; init; }
}
