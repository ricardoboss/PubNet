using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using PubNet.API.DTO.Authentication;
using PubNet.API.Interfaces;
using PubNet.API.Services;
using PubNet.Database;
using PubNet.Database.Models;

namespace PubNet.API.Tests;

/// <summary>
/// Wires up the services under test against an in-memory database.
/// </summary>
internal sealed class TestEnvironment : IDisposable
{
	public TestEnvironment()
	{
		Db = new TestPubNetContext(new DbContextOptionsBuilder<PubNetContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options);

		Passwords = new(new PasswordHasher<Author>(), NullLogger<PasswordManager>.Instance);

		Registration = new AuthorRegistrationService(Db, Passwords);

		Onboarding = new OnboardingService(Db, Registration);

		PasswordResets = new PasswordResetService(Db, Passwords, NullLogger<PasswordResetService>.Instance);
	}

	public PubNetContext Db { get; }

	public PasswordManager Passwords { get; }

	public IAuthorRegistrationService Registration { get; }

	public IOnboardingService Onboarding { get; }

	public IPasswordResetService PasswordResets { get; }

	public static RegisterRequestDto ValidRequest()
	{
		return new()
		{
			Username = "admin",
			Name = "Admin",
			Email = "admin@example.com",
			Password = "hunter2",
		};
	}

	/// <param name="password">Only needed by tests which have to pass a password confirmation.</param>
	public async Task<Author> AddAuthorAsync(string username, Role role, string? password = null)
	{
		var author = new Author
		{
			UserName = username,
			Email = $"{username}@example.com",
			Name = username,
			RegisteredAtUtc = DateTimeOffset.UtcNow,
			Role = role,
		};

		if (password is not null)
			author.PasswordHash = await Passwords.GenerateHashAsync(author, password);

		Db.Authors.Add(author);
		await Db.SaveChangesAsync();

		return author;
	}

	public async Task<string?> GetSettingAsync(string key)
	{
		return (await Db.Settings.SingleOrDefaultAsync(s => s.Key == key))?.Value;
	}

	/// <inheritdoc />
	public void Dispose()
	{
		Db.Dispose();
	}
}
