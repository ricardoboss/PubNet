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
	public void TestLoginMapsEmailNotFoundToInvalidCredentials()
	{
		var api = new EmailNotFoundErrorDto { Error = new() { Code = "email-not-found", Message = "nope" } };

		var e = Assert.ThrowsAsync<InvalidLoginCredentialsException>(
			() => Service(api).LoginAsync("someone@example.test", "hunter2"));

		Assert.Multiple(() =>
		{
			Assert.That(e?.InnerException, Is.SameAs(api));
			// deployments are internal, so the message deliberately says which of the two it was
			Assert.That(e?.Message, Does.Contain("someone@example.test"));
		});
	}

	[Test]
	public void TestLoginMapsInvalidPasswordToInvalidCredentials()
	{
		var api = new InvalidPasswordErrorDto { Error = new() { Code = "invalid-password", Message = "nope" } };

		var e = Assert.ThrowsAsync<InvalidLoginCredentialsException>(
			() => Service(api).LoginAsync("someone@example.test", "hunter2"));

		Assert.Multiple(() =>
		{
			Assert.That(e?.InnerException, Is.SameAs(api));
			Assert.That(e?.Message, Does.Not.Contain("someone@example.test"));
		});
	}

	[Test]
	public void TestLoginMapsUnexpectedApiErrorsToUnexpectedResponse()
	{
		Assert.ThrowsAsync<UnexpectedResponseException>(
			() => Service(new ApiException("boom")).LoginAsync("someone@example.test", "hunter2"));
	}


	[Test]
	public void TestChangePasswordMapsInvalidPassword()
	{
		var api = new InvalidPasswordErrorDto
		{
			Error = new() { Code = "invalid-password", Message = "nope" }
		};

		var e = Assert.ThrowsAsync<InvalidPasswordException>(
			() => Service(api).ChangePasswordAsync("wrong-password", "new-password"));

		Assert.That(e?.InnerException, Is.SameAs(api));
	}

	[Test]
	public void TestChangePasswordMapsInvalidNewPassword()
	{
		var api = new InvalidNewPasswordErrorDto
		{
			Error = new() { Code = "invalid-new-password", Message = "nope" }
		};

		var e = Assert.ThrowsAsync<InvalidNewPasswordException>(
			() => Service(api).ChangePasswordAsync("hunter2", "hunter2"));

		Assert.That(e?.InnerException, Is.SameAs(api));
	}

	[Test]
	public void TestChangePasswordMapsUnexpectedApiErrors()
	{
		Assert.ThrowsAsync<UnexpectedResponseException>(
			() => Service(new ApiException("boom"))
				.ChangePasswordAsync("hunter2", "new-password"));
	}

	[Test]
	public async Task TestChangePasswordSucceeds()
	{
		var adapter = new SuccessfulRequestAdapter();
		var service = new ApiAuthenticationService(
			new PubNetApiClient(adapter),
			new Mock<ILoginTokenStorage>().Object,
			NullLogger<ApiAuthenticationService>.Instance);

		await service.ChangePasswordAsync("hunter2", "new-password");
	}
}
