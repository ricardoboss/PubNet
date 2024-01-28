namespace PubNet.API.DTO.Packages.Dart.Spec;

public class DartNewVersionDto
{
	public required string Url { get; init; }

	public required Dictionary<string, string> Fields { get; init; } = new();
}
