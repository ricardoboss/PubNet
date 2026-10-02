using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using PubNet.API.Controllers;
using PubNet.API.DTO.Authentication;
using PubNet.API.Services;
using PubNet.Database.Models;

namespace PubNet.API.Tests;

public class ChangePasswordControllerTests
{
	private const string Password = "hunter2";
	private const string NewPassword = "new-password";

	[Test]
	public async Task ChangePassword_Succeeds_WithCorrectCurrentPassword()
	{
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("someone", Role.Default, Password);
		var originalHash = author.PasswordHash;

		var result = await ChangePasswordAsync(env, author, Password, NewPassword);

		Assert.That(result, Is.InstanceOf<OkResult>());

		var updatedAuthor = await env.Db.Authors.FindAsync(author.Id);

		Assert.That(updatedAuthor, Is.Not.Null);
		Assert.That(updatedAuthor!.PasswordHash, Is.Not.EqualTo(originalHash));

		Assert.That(
			await env.Passwords.IsValid(env.Db, updatedAuthor, NewPassword),
			Is.True);
	}

	[Test]
	public async Task ChangePassword_Rejects_WhenCurrentPasswordIsInvalid()
	{
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("someone", Role.Default, Password);

		var result = await ChangePasswordAsync(
			env,
			author,
			"wrong-password",
			NewPassword);

		Assert.That(
			(result as ObjectResult)?.StatusCode,
			Is.EqualTo(PubNetStatusCodes.Status461InvalidPassword));
	}

	[Test]
	public async Task ChangePassword_Rejects_WhenNewPasswordIsEmpty()
	{
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("someone", Role.Default, Password);

		var result = await ChangePasswordAsync(
			env,
			author,
			Password,
			"");

		Assert.That(
			(result as ObjectResult)?.StatusCode,
			Is.EqualTo(PubNetStatusCodes.Status467InvalidNewPassword));
	}

	[Test]
	public async Task ChangePassword_Rejects_WhenNewPasswordIsSameAsCurrentPassword()
	{
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("someone", Role.Default, Password);

		var result = await ChangePasswordAsync(
			env,
			author,
			Password,
			Password);

		Assert.That(
			(result as ObjectResult)?.StatusCode,
			Is.EqualTo(PubNetStatusCodes.Status467InvalidNewPassword));
	}

	private static Task<IActionResult> ChangePasswordAsync(
		TestEnvironment env,
		Author author,
		string oldPassword,
		string newPassword)
	{
		var controller = new AuthenticationController(
			null!,
			env.Db,
			env.Passwords,
			null!,
			env.Registration,
			env.Onboarding,
			null!,
			env.PasswordResets,
			NullLogger<AuthenticationController>.Instance)
		{
			ControllerContext = new()
			{
				HttpContext = new DefaultHttpContext(),
			},
		};

		var context = new ApplicationRequestContext
		{
			Author = author,
		};

		return controller.ChangePassword(
			context,
			new ChangePasswordRequestDto
			{
				OldPassword = oldPassword,
				NewPassword = newPassword,
			});
	}
}
