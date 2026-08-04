using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PubNet.API.Interfaces;
using PubNet.Database;
using PubNet.Database.Models;

namespace PubNet.API.Services;

public class PasswordResetService(PubNetContext db, PasswordManager passwordManager, ILogger<PasswordResetService> logger)
	: IPasswordResetService
{
	/// <summary>
	/// How long a reset link stays redeemable. Long enough to survive greylisting and a coffee break, short
	/// enough that a forgotten mailbox does not hold a working credential for days.
	/// </summary>
	public static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(1);

	/// <inheritdoc />
	public async Task<string> GenerateTokenAsync(Author author, CancellationToken cancellationToken = default)
	{
		var token = Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(32));
		var now = DateTimeOffset.UtcNow;

		db.PasswordResetTokens.Add(new()
		{
			AuthorId = author.Id,
			Author = author,
			TokenHash = HashToken(token),
			CreatedAtUtc = now,
			ExpiresAtUtc = now + TokenLifetime,
		});

		await db.SaveChangesAsync(cancellationToken);

		return token;
	}

	/// <inheritdoc />
	public async Task<Author> ResetPasswordAsync(string token, string password,
		CancellationToken cancellationToken = default)
	{
		var tokenHash = HashToken(token);

		var resetToken = await db.PasswordResetTokens
			.Include(t => t.Author)
			.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

		var now = DateTimeOffset.UtcNow;

		if (resetToken is null || resetToken.ConsumedAtUtc is not null || resetToken.ExpiresAtUtc <= now)
			throw new InvalidPasswordResetTokenException();

		var author = resetToken.Author;

		author.PasswordHash = await passwordManager.GenerateHashAsync(author, password, cancellationToken);

		// whoever holds the reset link owns the mailbox, so a lockout from old failed attempts is moot
		author.AccessFailedCount = 0;

		var outstandingTokens = await db.PasswordResetTokens
			.Where(t => t.AuthorId == author.Id && t.ConsumedAtUtc == null)
			.ToListAsync(cancellationToken);

		foreach (var outstandingToken in outstandingTokens)
			outstandingToken.ConsumedAtUtc = now;

		await db.SaveChangesAsync(cancellationToken);

		logger.LogInformation("Password was reset for {@Author}", author);

		return author;
	}

	private static string HashToken(string token) =>
		Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
