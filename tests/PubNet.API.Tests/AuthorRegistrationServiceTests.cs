using PubNet.API.DTO;
using PubNet.API.Services;
using PubNet.Database.Models;

namespace PubNet.API.Tests;

public class AuthorRegistrationServiceTests
{
	[Test]
	public async Task RegisterAsync_CreatesAnAuthor_WithTheGivenRole()
	{
		using var env = new TestEnvironment();

		var author = await env.Registration.RegisterAsync(TestEnvironment.ValidRequest(), Role.Default);

		Assert.Multiple(() =>
		{
			Assert.That(author.Role, Is.EqualTo(Role.Default));
			Assert.That(author.PasswordHash, Is.Not.Null);
			Assert.That(author.Inactive, Is.False);
		});
	}

	[Test]
	public void RegisterAsync_Throws_ForAnIncompleteRequest()
	{
		using var env = new TestEnvironment();

		var request = TestEnvironment.ValidRequest();
		request.Password = null;

		var e = Assert.ThrowsAsync<AuthorRegistrationException>(
			() => env.Registration.RegisterAsync(request, Role.Default));

		Assert.That(e?.Code, Is.EqualTo("missing-values"));
	}

	// the duplicate username and e-mail checks are not covered here: they use Npgsql's ILike, which the
	// in-memory provider cannot translate once there is a row to evaluate
}
