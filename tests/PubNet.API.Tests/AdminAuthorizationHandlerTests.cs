using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging.Abstractions;
using PubNet.API.Authorization;
using PubNet.API.Services;
using PubNet.Database.Models;

namespace PubNet.API.Tests;

public class AdminAuthorizationHandlerTests
{
	[Test]
	public async Task Succeeds_ForAnAdmin()
	{
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("admin", Role.Admin);

		Assert.That(await IsAuthorizedAsync(env, PrincipalFor(author.Id)), Is.True);
	}

	[Test]
	public async Task Fails_ForARegularAuthor()
	{
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("someone", Role.Default);

		Assert.That(await IsAuthorizedAsync(env, PrincipalFor(author.Id)), Is.False);
	}

	[Test]
	public async Task Fails_ForAnAnonymousCaller()
	{
		using var env = new TestEnvironment();

		Assert.That(await IsAuthorizedAsync(env, new ClaimsPrincipal(new ClaimsIdentity())), Is.False);
	}

	[Test]
	public async Task Fails_ForATokenOfADeletedAuthor()
	{
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("admin", Role.Admin);
		var principal = PrincipalFor(author.Id);

		env.Db.Authors.Remove(author);
		await env.Db.SaveChangesAsync();

		Assert.That(await IsAuthorizedAsync(env, principal), Is.False);
	}

	[Test]
	public async Task Fails_AfterADemotion()
	{
		// the role is not carried in the token, so losing it takes effect immediately
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("admin", Role.Admin);
		var principal = PrincipalFor(author.Id);

		author.Role = Role.Default;
		await env.Db.SaveChangesAsync();

		Assert.That(await IsAuthorizedAsync(env, principal), Is.False);
	}

	private static ClaimsPrincipal PrincipalFor(int authorId)
	{
		return new(new ClaimsIdentity([new Claim("id", authorId.ToString())], "Test"));
	}

	private static async Task<bool> IsAuthorizedAsync(TestEnvironment env, ClaimsPrincipal user)
	{
		var requirement = new AdminRequirement();
		var context = new AuthorizationHandlerContext([requirement], user, null);

		var handler = new AdminAuthorizationHandler(env.Db, new ApplicationRequestContext(),
			NullLogger<AdminAuthorizationHandler>.Instance);

		await handler.HandleAsync(context);

		return context.HasSucceeded;
	}
}
