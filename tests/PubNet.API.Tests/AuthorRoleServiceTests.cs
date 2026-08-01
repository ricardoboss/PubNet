using PubNet.API.Services;
using PubNet.Database.Models;

namespace PubNet.API.Tests;

public class AuthorRoleServiceTests
{
	[Test]
	public async Task IsLastAdminAsync_IsFalse_ForARegularAuthor()
	{
		using var env = new TestEnvironment();

		await env.AddAuthorAsync("admin", Role.Admin);
		var author = await env.AddAuthorAsync("someone", Role.Default);

		Assert.That(await env.Roles.IsLastAdminAsync(author), Is.False);
	}

	[Test]
	public async Task IsLastAdminAsync_IsTrue_ForTheOnlyAdmin()
	{
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("admin", Role.Admin);
		await env.AddAuthorAsync("someone", Role.Default);

		Assert.That(await env.Roles.IsLastAdminAsync(author), Is.True);
	}

	[Test]
	public async Task IsLastAdminAsync_IsFalse_WhenAnotherAdminExists()
	{
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("admin", Role.Admin);
		await env.AddAuthorAsync("second", Role.Admin);

		Assert.That(await env.Roles.IsLastAdminAsync(author), Is.False);
	}

	[Test]
	public async Task SetRoleAsync_Promotes()
	{
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("someone", Role.Default);

		await env.Roles.SetRoleAsync(author, Role.Admin);

		Assert.That(author.Role, Is.EqualTo(Role.Admin));
	}

	[Test]
	public async Task SetRoleAsync_Throws_WhenDemotingTheLastAdmin()
	{
		// an instance without admins could never be administered again, since onboarding cannot be repeated
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("admin", Role.Admin);

		Assert.ThrowsAsync<LastAdminException>(() => env.Roles.SetRoleAsync(author, Role.Default));

		Assert.That(author.Role, Is.EqualTo(Role.Admin));
	}

	[Test]
	public async Task SetRoleAsync_DemotesAnAdmin_WhenAnotherOneRemains()
	{
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("admin", Role.Admin);
		await env.AddAuthorAsync("second", Role.Admin);

		await env.Roles.SetRoleAsync(author, Role.Default);

		Assert.That(author.Role, Is.EqualTo(Role.Default));
	}
}
