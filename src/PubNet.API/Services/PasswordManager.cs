using System.Security.Authentication;
using Microsoft.AspNetCore.Identity;
using PubNet.Database;
using PubNet.Database.Models;

namespace PubNet.API.Services;

public class PasswordManager(IPasswordHasher<Author> passwordHasher, ILogger<PasswordManager> logger)
{
	private static InvalidCredentialException PasswordVerificationFailed => new("Password verification failed");

	public Task<string> GenerateHashAsync(Author author, string password, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		return Task.FromResult(passwordHasher.HashPassword(author, password));
	}

	public async Task<bool> IsValid(PubNetContext db, Author author, string? password,
		CancellationToken cancellationToken = default)
	{
		try
		{
			await ThrowForInvalid(db, author, password, cancellationToken);

			return true;
		}
		catch (InvalidCredentialException)
		{
			return false;
		}
	}

	public async Task ThrowForInvalid(PubNetContext db, Author author, string? password,
		CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		// TODO: check author.LockoutEnabled and respond with Retry-After value from author.LockoutEnds

		if (password is null || author.PasswordHash is null)
			throw PasswordVerificationFailed;

		var result = passwordHasher.VerifyHashedPassword(author, author.PasswordHash, password);
		if (result == PasswordVerificationResult.SuccessRehashNeeded)
		{
			author.PasswordHash = passwordHasher.HashPassword(author, password);

			logger.LogInformation("Rehashed password for {@Author}", author);
		}
		else if (result != PasswordVerificationResult.Success)
		{
			logger.LogInformation("Wrong password for {@Author}", author);

			author.AccessFailedCount++;
			await db.SaveChangesAsync(cancellationToken);

			// TODO: check if AccessFailedCount crossed a threshold and lock user out for some time

			throw PasswordVerificationFailed;
		}

		author.AccessFailedCount = 0;
		await db.SaveChangesAsync(cancellationToken);
	}
}
