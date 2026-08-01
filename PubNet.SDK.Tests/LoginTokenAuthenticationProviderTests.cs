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

	// MIND: these two cases must stub an available token. With a null token no header is attached either
	// way, so the assertions would hold even if the exemption logic were inverted.

	[Test]
	[TestCase(Method.POST, "https://example.test/authentication/login")]
	[TestCase(Method.POST, "https://example.test/authentication/register")]
	[TestCase(Method.GET, "https://example.test/authentication/registrations-enabled")]
	[TestCase(Method.POST, "https://example.test/api/authentication/login")] // additional case with "/api" infix
	[TestCase(Method.POST, "https://example.test/Authentication/Login")] // different casing
	public async Task TestOmitsAuthenticationForSpecialEndpoints(Method method, string endpoint)
	{
		var loginTokenStorageMock = new Mock<ILoginTokenStorage>();

		loginTokenStorageMock
			.Setup(s => s.GetTokenAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync("token");

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

		Assert.That(request.Headers.ContainsKey("Authorization"), Is.False,
			"an exempt endpoint must not be given a token even when one is available");

		// the exemption should short-circuit before the token store is consulted at all
		loginTokenStorageMock.Verify(s => s.GetTokenAsync(It.IsAny<CancellationToken>()), Times.Never);
	}

	[Test]
	[TestCase(Method.GET, "https://example.test/authentication/login")]
	[TestCase(Method.GET, "https://example.test/authentication/register")]
	[TestCase(Method.POST, "https://example.test/authentication/registrations-enabled")]
	[TestCase(Method.GET, "https://example.test/authentication/self")]
	[TestCase(Method.POST, "https://example.test/authentication/self")]
	public async Task TestAuthenticatesSpecialEndpointsWhenMethodDiffers(Method method, string endpoint)
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
			"the exemption only covers one method, so this request must still be authenticated");

		loginTokenStorageMock.VerifyAll();
	}
}
