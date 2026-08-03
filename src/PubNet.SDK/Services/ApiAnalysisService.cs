using Microsoft.Kiota.Abstractions;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Exceptions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Models;
using PubNet.SDK.Generated.Packages.Item.Versions.Item.Analysis;

namespace PubNet.SDK.Services;

internal sealed class ApiAnalysisService(PubNetApiClient apiClient) : IAnalysisService
{
	public async Task<PackageVersionAnalysisDto?> GetAnalysisForPackageVersionAsync(string name, string version,
		bool includeReadme, CancellationToken cancellationToken = default)
	{
		try
		{
			return await apiClient.Packages[name].Versions[version].Analysis
				.GetAsync(r => r.QueryParameters.IncludeReadme = includeReadme, cancellationToken: cancellationToken);
		}
		catch (PackageVersionNotFoundErrorDto)
		{
			// a version that has not been analysed yet simply has no analysis, which is not an error
			return null;
		}
		catch (PackageVersionAnalysisDto401Error e)
		{
			throw new AuthenticationRequiredException(e);
		}
		catch (ApiException e)
		{
			throw new UnexpectedResponseException(e);
		}
	}
}
