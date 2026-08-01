using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using PubNet.API.DTO;
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

		Registration = new AuthorRegistrationService(Db,
			new(new PasswordHasher<Author>(), NullLogger<PasswordManager>.Instance));

		Onboarding = new OnboardingService(Db, Registration);
	}

	public PubNetContext Db { get; }

	public IAuthorRegistrationService Registration { get; }

	public IOnboardingService Onboarding { get; }

	public static RegisterRequest ValidRequest()
	{
		return new()
		{
			Username = "admin",
			Name = "Admin",
			Email = "admin@example.com",
			Password = "hunter2",
		};
	}

	public async Task<Author> AddAuthorAsync(string username, Role role)
	{
		var author = new Author
		{
			UserName = username,
			Email = $"{username}@example.com",
			Name = username,
			RegisteredAtUtc = DateTimeOffset.UtcNow,
			Role = role,
		};

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
