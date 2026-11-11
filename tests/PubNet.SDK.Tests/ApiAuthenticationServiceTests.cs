using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Exceptions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Models;
using PubNet.SDK.Services;

namespace PubNet.SDK.Tests;

public class ApiAuthenticationServiceTests
{
	private static ApiAuthenticationService Service(Exception thrownByTheApi)
	{
		var adapter = new ThrowingRequestAdapter(thrownByTheApi);

		return new(new PubNetApiClient(adapter), new Mock<ILoginTokenStorage>().Object,
			NullLogger<ApiAuthenticationService>.Instance);
	}

	// The generated error DTOs never escape the SDK. Callers see PubNetSdkException subclasses instead, so
	// catching the DTOs further up (as the login form used to) is dead code.
	[Test]
	public async Task TestLoginMapsEmailNotFoundToInvalidCredentials()
	{
		var api = new EmailNotFoundErrorDto { Error = new() { Code = "email-not-found", Message = "nope" } };

		var e = await Assert.ThrowsAsync<InvalidLoginCredentialsException>(
			() => Service(api).LoginAsync("someone@example.test", "hunter2"));

		Assert.Multiple(() =>
		{
			Assert.That(e?.InnerException, Is.SameAs(api));
			// deployments are internal, so the message deliberately says which of the two it was
			Assert.That(e?.Message, Does.Contain("someone@example.test"));
		});
	}

	[Test]
	public async Task TestLoginMapsInvalidPasswordToInvalidCredentials()
	{
		var api = new InvalidPasswordErrorDto { Error = new() { Code = "invalid-password", Message = "nope" } };

		var e = await Assert.ThrowsAsync<InvalidLoginCredentialsException>(
			() => Service(api).LoginAsync("someone@example.test", "hunter2"));

		Assert.Multiple(() =>
		{
			Assert.That(e?.InnerException, Is.SameAs(api));
			Assert.That(e?.Message, Does.Not.Contain("someone@example.test"));
		});
	}

	[Test]
	public async Task TestLoginMapsUnexpectedApiErrorsToUnexpectedResponse()
	{
		await Assert.ThrowsAsync<UnexpectedResponseException>(
			() => Service(new ApiException("boom")).LoginAsync("someone@example.test", "hunter2"));
	}
}
