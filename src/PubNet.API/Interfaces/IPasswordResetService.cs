using PubNet.API.Services;
using PubNet.Database.Models;

namespace PubNet.API.Interfaces;

public interface IPasswordResetService
{
	/// <summary>
	/// Creates a new, single-use password reset token for the given author. The returned token is meant to be
	/// sent to the author's e-mail address; only a hash of it is stored.
	/// </summary>
	Task<string> GenerateTokenAsync(Author author, CancellationToken cancellationToken = default);

	/// <summary>
	/// Redeems a token created by <see cref="GenerateTokenAsync"/> and gives the author it belongs to the new
	/// password. Consumes every outstanding token of that author, so a reset link only ever works once.
	/// </summary>
	/// <returns>The author whose password was reset.</returns>
	/// <exception cref="InvalidPasswordResetTokenException">
	/// If the token is unknown, expired or has already been used.
	/// </exception>
	Task<Author> ResetPasswordAsync(string token, string password, CancellationToken cancellationToken = default);
}
