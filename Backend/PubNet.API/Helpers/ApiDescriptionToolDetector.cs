using System.Reflection;

namespace PubNet.API.Helpers;

/// <summary>
/// Tries to detect if the API is only started to generate the OpenAPI document.
/// </summary>
public static class ApiDescriptionToolDetector
{

	/// <summary>
	/// Tries to detect if the API is only started to generate the OpenAPI document.
	/// </summary>
	/// <remarks>
	/// The check relies on the tool being run from an entry assembly with the name containing "GetDocument.Insider".
	/// So, this is quite fragile, but the only way right now.
	/// </remarks>
	/// <returns>True if the API is only started to generate the OpenAPI document, false otherwise.</returns>
	public static bool IsToolInvocation()
	{
		var assembly = Assembly.GetEntryAssembly();
		if (assembly is null)
			return false;

		var entryAssemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
		if (entryAssemblyName is null)
			return false;

		var ranFromTool = entryAssemblyName.Contains("GetDocument.Insider", StringComparison.OrdinalIgnoreCase);

		return ranFromTool;
	}
}
