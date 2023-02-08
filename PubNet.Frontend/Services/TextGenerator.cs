using PubNet.API.DTO;

namespace PubNet.Frontend.Services;

public static class TextGenerator
{
	public static string PubspecVersionText(string name, string version, bool mirrored, string? httpBaseAddress)
	{
		if (mirrored)
			return $"{name}: ^{version}";

		return $@"{name}:
  hosted:
    url: {httpBaseAddress}
    name: {name}
    version: ^{version}";
	}


	public static string HrefToDependency(string name) => "/packages/" + name;


	public static string bla(PackageVersionAnalysisDto? AnalysisDto, PackageVersionDto? PackageVersionDto)
	{
		var result = AnalysisDto?.Formatted switch
		{
			true => "Formatted",
			false => "not formatted",
			_ => PackageVersionDto is not null ? "not analyzed" : string.Empty,
		};

		return result;
	}
}
