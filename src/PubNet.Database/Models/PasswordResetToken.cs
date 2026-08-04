using System.ComponentModel.DataAnnotations;

namespace PubNet.Database.Models;

/// <summary>
/// A single-use, expiring token with which an author can set a new password without knowing the old one.
/// Only a hash of the token is stored; the token itself only ever travels in the reset e-mail.
/// </summary>
public class PasswordResetToken
{
	[Key] public int Id { get; set; }

	public int AuthorId { get; set; }

	public Author Author { get; set; } = null!;

	/// <summary>
	/// Hex-encoded SHA-256 hash of the token sent to the author.
	/// </summary>
	[Required, MaxLength(64)]
	public string TokenHash { get; set; } = string.Empty;

	public DateTimeOffset CreatedAtUtc { get; set; }

	public DateTimeOffset ExpiresAtUtc { get; set; }

	/// <summary>
	/// When the token was used up, either by resetting the password with it or because another token for the
	/// same author was used. <see langword="null"/> while it can still be redeemed.
	/// </summary>
	public DateTimeOffset? ConsumedAtUtc { get; set; }
}
