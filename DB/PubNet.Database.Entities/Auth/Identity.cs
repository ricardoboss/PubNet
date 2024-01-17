using Microsoft.EntityFrameworkCore;

namespace PubNet.Database.Entities.Auth;

/// <summary>
/// The identity enables an <see cref="Author"/> to authenticate with the system.
/// </summary>
[EntityTypeConfiguration<IdentityConfiguration, Identity>]
public class Identity
{
	public Guid Id { get; set; }

	public Guid AuthorId { get; set; }

	public Author Author { get; set; } = null!;

	public string Email { get; set; } = null!;

	public string PasswordHash { get; set; } = null!;
}
