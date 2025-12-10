namespace PubNet.Frontend.Services;

public class TextGenerator(BaseAddressProvider baseAddressProvider)
{
	public string PubspecVersionText(string name, string version, bool mirrored)
	{
		if (mirrored)
			return $"{name}: ^{version}";

		return $"""
				{name}:
				  hosted:
				    url: {baseAddressProvider.BaseUri}
				    name: {name}
				    version: ^{version}
				""";
	}


	public static string HrefToDependency(string name) => "/packages/" + name;

	public static string VersionToUrlSegment(string version) => version.Replace('.', '_');

	public static string UrlSegmentToVersion(string urlSegment) => urlSegment.Replace('_', '.');
}
