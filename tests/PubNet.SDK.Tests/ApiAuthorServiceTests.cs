using Microsoft.Kiota.Abstractions;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Exceptions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Authors;
using PubNet.SDK.Generated.Authors.Item;
using PubNet.SDK.Generated.Authors.Item.Delete;
using PubNet.SDK.Generated.Models;
using PubNet.SDK.Services;

namespace PubNet.SDK.Tests;

public class ApiAuthorServiceTests
{
	private static ApiAuthorService Service(Exception thrownByTheApi) => new(
		new PubNetApiClient(new ThrowingRequestAdapter(thrownByTheApi)),
		new Mock<IAuthenticationService>().Object);

	[Test]
	public async Task TestGetAuthorTreatsNotFoundAsNoAuthor()
	{
		var author = await Service(new AuthorNotFoundErrorDto()).GetAuthorAsync("nobody");

		Assert.That(author, Is.Null);
	}

	[Test]
	public void TestGetAuthorMapsUnauthenticated()
	{
		Assert.ThrowsAsync<AuthenticationRequiredException>(
			() => Service(new AuthorDto401Error()).GetAuthorAsync("someone"));
	}

	[Test]
	public void TestGetAuthorsMapsUnauthenticated()
	{
		Assert.ThrowsAsync<AuthenticationRequiredException>(
			() => Service(new AuthorsResponseDto401Error()).GetAuthorsAsync());
	}

	[Test]
	public void TestUpdateAuthorMapsForbidden()
	{
		Assert.ThrowsAsync<UnauthorizedException>(
			() => Service(new ForbiddenErrorDto()).UpdateAuthorAsync("someone", new()));
	}

	[Test]
	public void TestDeleteAuthorMapsInvalidPassword()
	{
		Assert.ThrowsAsync<InvalidPasswordException>(
			() => Service(new InvalidPasswordErrorDto()).DeleteAuthorAsync("someone", new()));
	}

	[Test]
	public void TestDeleteAuthorMapsLastAdmin()
	{
		Assert.ThrowsAsync<LastAdminException>(
			() => Service(new LastAdminErrorDto()).DeleteAuthorAsync("someone", new()));
	}

	[Test]
	public void TestDeleteAuthorMapsUnauthenticated()
	{
		Assert.ThrowsAsync<AuthenticationRequiredException>(
			() => Service(new SuccessMessageDto401Error()).DeleteAuthorAsync("someone", new()));
	}

	// anything the SDK does not model must still surface as an SDK exception, never as a generated type
	[Test]
	public void TestMapsUnexpectedApiErrors()
	{
		Assert.ThrowsAsync<UnexpectedResponseException>(
			() => Service(new ApiException("boom")).GetAuthorAsync("someone"));
	}
}
