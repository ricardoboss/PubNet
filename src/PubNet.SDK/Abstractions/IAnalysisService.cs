using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Abstractions;

public interface IAnalysisService
{
	/// <summary>
	/// Fetches the analysis of a package version.
	/// </summary>
	/// <returns>The analysis, or <see langword="null"/> if the version has none (yet).</returns>
	/// <exception cref="Exceptions.AuthenticationRequiredException">Not authenticated.</exception>
	/// <exception cref="Exceptions.UnexpectedResponseException">The API answered in a way the SDK does not model.</exception>
	Task<PackageVersionAnalysisDto?> GetAnalysisForPackageVersionAsync(string name, string version, bool includeReadme, CancellationToken cancellationToken = default);
}
