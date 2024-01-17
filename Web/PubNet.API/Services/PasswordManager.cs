using System.Security.Authentication;
using Microsoft.AspNetCore.Identity;
using PubNet.Database.Context;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Services;

public class PasswordManager
{
	private readonly ILogger<PasswordManager> _logger;

	private readonly IPasswordHasher<Identity> _passwordHasher;

	public PasswordManager(IPasswordHasher<Identity> passwordHasher, ILogger<PasswordManager> logger)
	{
		_passwordHasher = passwordHasher;
		_logger = logger;
	}

	private static InvalidCredentialException PasswordVerificationFailed => new("Password verification failed");

	public Task<string> GenerateHashAsync(Identity identity, string password, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		return Task.FromResult(_passwordHasher.HashPassword(identity, password));
	}

	public async Task<bool> IsValid(PubNetContext db, Identity identity, string? password,
		CancellationToken cancellationToken = default)
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

	public async Task ThrowForInvalid(PubNetContext db, Identity identity, string? password,
		CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		// TODO: check identity.LockoutEnabled and respond with Retry-After value from identity.LockoutEnds

		if (password is null || identity.PasswordHash is null)
			throw PasswordVerificationFailed;

		var result = _passwordHasher.VerifyHashedPassword(identity, identity.PasswordHash, password);
		if (result == PasswordVerificationResult.SuccessRehashNeeded)
		{
			identity.PasswordHash = _passwordHasher.HashPassword(identity, password);

			_logger.LogInformation("Rehashed password for {@Author}", identity);
		}
		else if (result != PasswordVerificationResult.Success)
		{
			_logger.LogInformation("Wrong password for {@Author}", identity);

			throw new NotImplementedException("Access failed count increment");
			// identity.AccessFailedCount++;
			await db.SaveChangesAsync(cancellationToken);

			// TODO: check if AccessFailedCount crossed a threshold and lock user out for some time

			throw PasswordVerificationFailed;
		}

		// identity.AccessFailedCount = 0;
		await db.SaveChangesAsync(cancellationToken);
	}
}
