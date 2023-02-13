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

	public static string VersionToUrlSegment(string version) => version.Replace('.', '_');

	public static string UrlSegmentToVersion(string urlSegment) => urlSegment.Replace('_', '.');
}
