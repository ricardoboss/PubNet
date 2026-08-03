using Microsoft.Kiota.Abstractions;
using PubNet.SDK.Exceptions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Models;
using PubNet.SDK.Generated.Packages.Item.Versions.Item.Analysis;
using PubNet.SDK.Services;

namespace PubNet.SDK.Tests;

public class ApiAnalysisServiceTests
{
	private static ApiAnalysisService Service(Exception thrownByTheApi) =>
		new(new PubNetApiClient(new ThrowingRequestAdapter(thrownByTheApi)));

	[Test]
	public async Task TestTreatsNotFoundAsNoAnalysis()
	{
		var analysis = await Service(new PackageVersionNotFoundErrorDto())
			.GetAnalysisForPackageVersionAsync("pkg", "1.0.0", includeReadme: false);

		Assert.That(analysis, Is.Null);
	}

	[Test]
	public void TestMapsUnauthenticated()
	{
		Assert.ThrowsAsync<AuthenticationRequiredException>(
			() => Service(new PackageVersionAnalysisDto401Error())
				.GetAnalysisForPackageVersionAsync("pkg", "1.0.0", includeReadme: false));
	}

	[Test]
	public void TestMapsUnexpectedApiErrors()
	{
		Assert.ThrowsAsync<UnexpectedResponseException>(
			() => Service(new ApiException("boom"))
				.GetAnalysisForPackageVersionAsync("pkg", "1.0.0", includeReadme: false));
	}
}
