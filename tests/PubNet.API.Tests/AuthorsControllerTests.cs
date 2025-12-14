using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PubNet.API.Controllers;
using PubNet.API.DTO.Errors;
using PubNet.API.Services;
using PubNet.Database.Models;

namespace PubNet.API.Tests;

public class AuthorsControllerTests
{
	private const string Password = "hunter2";

	[Test]
	public async Task Delete_Refuses_ForTheLastAdmin()
	{
		// onboarding stays closed once completed, so an instance losing its last admin cannot get another
		// one without editing the database by hand
		using var env = new TestEnvironment();

		var admin = await env.AddAuthorAsync("admin", Role.Admin, Password);

		var result = await DeleteSelfAsync(env, admin);

		await Assert.MultipleAsync(async () =>
		{
			Assert.That((result as ObjectResult)?.StatusCode, Is.EqualTo(PubNetStatusCodes.Status480LastAdmin));
			Assert.That(ErrorCodeOf(result), Is.EqualTo("last-admin"));
			Assert.That(await env.Db.Authors.AnyAsync(a => a.Id == admin.Id), Is.True);
		});
	}

	[Test]
	public async Task Delete_Succeeds_WhenAnotherAdminRemains()
	{
		using var env = new TestEnvironment();

		var admin = await env.AddAuthorAsync("admin", Role.Admin, Password);
		await env.AddAuthorAsync("second", Role.Admin, Password);

		var result = await DeleteSelfAsync(env, admin);

		await Assert.MultipleAsync(async () =>
		{
			Assert.That(result, Is.InstanceOf<OkObjectResult>());
			Assert.That(await env.Db.Authors.AnyAsync(a => a.Id == admin.Id), Is.False);
		});
	}

	[Test]
	public async Task Delete_Succeeds_ForTheLastRegularAuthor()
	{
		// only admins are load bearing; a regular account leaving cannot strand the instance
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("someone", Role.Default, Password);

		var result = await DeleteSelfAsync(env, author);

		await Assert.MultipleAsync(async () =>
		{
			Assert.That(result, Is.InstanceOf<OkObjectResult>());
			Assert.That(await env.Db.Authors.AnyAsync(a => a.Id == author.Id), Is.False);
		});
	}

	[Test]
	public async Task Delete_ChecksThePassword_BeforeTheLastAdminRule()
	{
		// the rule reports whether the instance has other admins, so it must not answer to a caller who
		// cannot confirm the account is theirs
		using var env = new TestEnvironment();

		var admin = await env.AddAuthorAsync("admin", Role.Admin, Password);

		var result = await DeleteSelfAsync(env, admin, "not-the-password");

		Assert.That((result as ObjectResult)?.StatusCode, Is.EqualTo(PubNetStatusCodes.Status461InvalidPassword));
	}

	private static Task<IActionResult> DeleteSelfAsync(TestEnvironment env, Author author,
		string password = Password)
	{
		var controller = new AuthorsController(NullLogger<AuthorsController>.Instance, env.Db, env.Passwords)
		{
			ControllerContext = new() { HttpContext = new DefaultHttpContext() },
		};

		// the author is already resolved, so the action does not go looking for a bearer token
		return controller.Delete(author.UserName, new(password), new() { Author = author });
	}

	private static string? ErrorCodeOf(IActionResult result)
	{
		return ((result as ObjectResult)?.Value as ErrorMessageDto)?.Error?.Code;
	}
}
