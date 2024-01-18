using System.Security.Authentication;
using Microsoft.AspNetCore.Identity;
using PubNet.Database.Context;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Services;

public class PasswordManager(IPasswordHasher<Identity> passwordHasher, ILogger<PasswordManager> logger)
{
	private static InvalidCredentialException PasswordVerificationFailed => new("Password verification failed");

	public Task<string> GenerateHashAsync(Identity identity, string password, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		return Task.FromResult(passwordHasher.HashPassword(identity, password));
	}

	public async Task<bool> IsValid(PubNetContext db, Identity identity, string? password, CancellationToken cancellationToken = default)
	{
		try
		{
			await ThrowForInvalid(db, identity, password, cancellationToken);

			return true;
		}
		catch (InvalidCredentialException)
		{
			return false;
		}
	}

	public async Task ThrowForInvalid(PubNetContext db, Identity identity, string? password, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		// TODO: check identity.LockoutEnabled and respond with Retry-After value from identity.LockoutEnds

		if (password is null || identity.PasswordHash is null)
			throw PasswordVerificationFailed;

		var result = passwordHasher.VerifyHashedPassword(identity, identity.PasswordHash, password);
		if (result == PasswordVerificationResult.SuccessRehashNeeded)
		{
			identity.PasswordHash = passwordHasher.HashPassword(identity, password);

			logger.LogInformation("Rehashed password for {@Author}", identity);

			await db.SaveChangesAsync(cancellationToken);
		}
		else if (result != PasswordVerificationResult.Success)
		{
			logger.LogInformation("Wrong password for {@Author}", identity);

			throw PasswordVerificationFailed;
		}
	}
}
