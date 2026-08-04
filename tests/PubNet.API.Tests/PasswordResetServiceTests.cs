using Microsoft.EntityFrameworkCore;
using PubNet.API.Services;
using PubNet.Database.Models;

namespace PubNet.API.Tests;

public class PasswordResetServiceTests
{
	[Test]
	public async Task GenerateTokenAsync_StoresOnlyAHash_WithAnExpiry()
	{
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("alice", Role.Default, "hunter2");

		var token = await env.PasswordResets.GenerateTokenAsync(author);

		var stored = await env.Db.PasswordResetTokens.SingleAsync();
		Assert.Multiple(() =>
		{
			Assert.That(token, Is.Not.Empty);
			Assert.That(stored.TokenHash, Is.Not.EqualTo(token));
			Assert.That(stored.AuthorId, Is.EqualTo(author.Id));
			Assert.That(stored.ExpiresAtUtc, Is.GreaterThan(DateTimeOffset.UtcNow));
			Assert.That(stored.ConsumedAtUtc, Is.Null);
		});
	}

	[Test]
	public async Task ResetPasswordAsync_SetsTheNewPassword_AndConsumesTheToken()
	{
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("alice", Role.Default, "hunter2");
		var token = await env.PasswordResets.GenerateTokenAsync(author);

		await env.PasswordResets.ResetPasswordAsync(token, "correct horse battery staple");

		Assert.Multiple(async () =>
		{
			Assert.That(await env.Passwords.IsValid(env.Db, author, "correct horse battery staple"), Is.True);
			Assert.That(await env.Passwords.IsValid(env.Db, author, "hunter2"), Is.False);
			Assert.That((await env.Db.PasswordResetTokens.SingleAsync()).ConsumedAtUtc, Is.Not.Null);
		});
	}

	[Test]
	public async Task ResetPasswordAsync_Throws_ForAnUnknownToken()
	{
		using var env = new TestEnvironment();

		await env.AddAuthorAsync("alice", Role.Default, "hunter2");

		Assert.ThrowsAsync<InvalidPasswordResetTokenException>(
			() => env.PasswordResets.ResetPasswordAsync("not-a-token", "irrelevant"));
	}

	[Test]
	public async Task ResetPasswordAsync_Throws_ForAnExpiredToken()
	{
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("alice", Role.Default, "hunter2");
		var token = await env.PasswordResets.GenerateTokenAsync(author);

		var stored = await env.Db.PasswordResetTokens.SingleAsync();
		stored.ExpiresAtUtc = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(1);
		await env.Db.SaveChangesAsync();

		Assert.ThrowsAsync<InvalidPasswordResetTokenException>(
			() => env.PasswordResets.ResetPasswordAsync(token, "irrelevant"));

		Assert.That(await env.Passwords.IsValid(env.Db, author, "hunter2"), Is.True);
	}

	[Test]
	public async Task ResetPasswordAsync_Throws_WhenTheTokenIsUsedTwice()
	{
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("alice", Role.Default, "hunter2");
		var token = await env.PasswordResets.GenerateTokenAsync(author);

		await env.PasswordResets.ResetPasswordAsync(token, "first reset");

		Assert.ThrowsAsync<InvalidPasswordResetTokenException>(
			() => env.PasswordResets.ResetPasswordAsync(token, "second reset"));

		Assert.That(await env.Passwords.IsValid(env.Db, author, "first reset"), Is.True);
	}

	[Test]
	public async Task ResetPasswordAsync_ConsumesEveryOutstandingToken_OfTheAuthor()
	{
		using var env = new TestEnvironment();

		var author = await env.AddAuthorAsync("alice", Role.Default, "hunter2");
		var firstToken = await env.PasswordResets.GenerateTokenAsync(author);
		var secondToken = await env.PasswordResets.GenerateTokenAsync(author);

		await env.PasswordResets.ResetPasswordAsync(secondToken, "new password");

		Assert.ThrowsAsync<InvalidPasswordResetTokenException>(
			() => env.PasswordResets.ResetPasswordAsync(firstToken, "should not work"));
	}

	[Test]
	public async Task ResetPasswordAsync_LeavesOtherAuthorsTokens_Alone()
	{
		using var env = new TestEnvironment();

		var alice = await env.AddAuthorAsync("alice", Role.Default, "hunter2");
		var bob = await env.AddAuthorAsync("bob", Role.Default, "hunter2");
		var aliceToken = await env.PasswordResets.GenerateTokenAsync(alice);
		var bobToken = await env.PasswordResets.GenerateTokenAsync(bob);

		await env.PasswordResets.ResetPasswordAsync(aliceToken, "alices new password");

		await env.PasswordResets.ResetPasswordAsync(bobToken, "bobs new password");

		Assert.That(await env.Passwords.IsValid(env.Db, bob, "bobs new password"), Is.True);
	}
}
