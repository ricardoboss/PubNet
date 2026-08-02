using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Kiota.Abstractions;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Services;

namespace PubNet.SDK.Tests;

public class LoginTokenAuthenticationProviderTests
{
	[Test]
	public async Task TestAuthenticatesRequests()
	{
		const string token = "token";
		var loginTokenStorageMock = new Mock<ILoginTokenStorage>();

		loginTokenStorageMock
			.Setup(s => s.GetTokenAsync())
			.ReturnsAsync(token)
			.Verifiable();

		var request = new RequestInformation
		{
			HttpMethod = Method.GET,
			URI = new("https://example.test/some-endpoint"),
		};

		var provider = new LoginTokenAuthenticationProvider(
			loginTokenStorageMock.Object,
			NullLogger<LoginTokenAuthenticationProvider>.Instance
		);

		await provider.AuthenticateRequestAsync(request);

		Assert.That(request.Headers["Authorization"].SingleOrDefault(), Is.EqualTo($"Bearer {token}"));

		loginTokenStorageMock.VerifyAll();
	}

	[Test]
	public async Task TestDoesNothingForMissingToken()
	{
		const string endpoint = "https://example.test/some-endpoint";
		var loginTokenStorageMock = new Mock<ILoginTokenStorage>();

		loginTokenStorageMock
			.Setup(s => s.GetTokenAsync())
			.ReturnsAsync((CancellationToken _) => null)
			.Verifiable();

		var request = new RequestInformation
		{
			HttpMethod = Method.GET,
			URI = new(endpoint),
		};

		var provider = new LoginTokenAuthenticationProvider(
			loginTokenStorageMock.Object,
			NullLogger<LoginTokenAuthenticationProvider>.Instance
		);

		await provider.AuthenticateRequestAsync(request);

		Assert.That(request.Headers.ContainsKey("Authorization"), Is.False);

		loginTokenStorageMock.VerifyAll();
	}

	/// <summary>
	/// There is no allow list of unauthenticated endpoints: the token goes out with every request that
	/// has one. The endpoints below are the ones that do not require authentication - they are
	/// <c>[AllowAnonymous]</c> on the server, so the header is simply ignored.
	/// </summary>
	[Test]
	[TestCase(Method.POST, "https://example.test/authentication/login")]
	[TestCase(Method.POST, "https://example.test/authentication/register")]
	[TestCase(Method.GET, "https://example.test/authentication/registrations-enabled")]
	[TestCase(Method.GET, "https://example.test/authentication/self")]
	[TestCase(Method.POST, "https://example.test/storage/upload")]
	[TestCase(Method.GET, "https://example.test/api/authentication/login")] // additional case with "/api" infix
	[TestCase(Method.POST, "https://example.test/Authentication/Login")] // different casing
	public async Task TestAuthenticatesEveryEndpointIncludingAnonymousOnes(Method method, string endpoint)
	{
		const string token = "token";
		var loginTokenStorageMock = new Mock<ILoginTokenStorage>();

		loginTokenStorageMock
			.Setup(s => s.GetTokenAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(token)
			.Verifiable();

		var request = new RequestInformation
		{
			HttpMethod = method,
			URI = new(endpoint),
		};

		var provider = new LoginTokenAuthenticationProvider(
			loginTokenStorageMock.Object,
			NullLogger<LoginTokenAuthenticationProvider>.Instance
		);

		await provider.AuthenticateRequestAsync(request);

		Assert.That(request.Headers["Authorization"].SingleOrDefault(), Is.EqualTo($"Bearer {token}"),
			"every request carries the token when one is available");

		loginTokenStorageMock.VerifyAll();
	}
}
