using Microsoft.EntityFrameworkCore;

namespace PubNet.Database.Entities.Auth;

/// <summary>
/// A token is used to authenticate an <see cref="Identity"/> with the system and determine its permissions.
/// </summary>
[EntityTypeConfiguration<TokenConfiguration, Token>]
public class Token
{
	public Guid Id { get; set; }

	public Guid IdentityId { get; set; }

	public virtual Identity Identity { get; set; } = null!;

	public string Value { get; set; } = null!;

	public string IpAddress { get; set; } = null!;

	public string UserAgent { get; set; } = null!;

	public string DeviceType { get; set; } = null!;

	public string Browser { get; set; } = null!;

	public string Platform { get; set; } = null!;

	public string Name { get; set; } = null!;

	public string[] Scopes { get; set; } = null!;

	public DateTimeOffset CreatedAtUtc { get; set; }

	public DateTimeOffset ExpiresAtUtc { get; set; }

	public DateTimeOffset? RevokedAtUtc { get; set; }

	public bool IsExpired => RevokedAtUtc.HasValue || DateTimeOffset.UtcNow > ExpiresAtUtc;
}
