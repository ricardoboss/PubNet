using Microsoft.EntityFrameworkCore;
using PubNet.Auth.Models;

namespace PubNet.Database.Entities.Auth;

/// <summary>
/// The identity enables an <see cref="Author"/> to authenticate with the system.
/// </summary>
[EntityTypeConfiguration<IdentityConfiguration, Identity>]
public class Identity
{
	public Guid Id { get; set; }

	public Guid AuthorId { get; set; }

	public virtual Author Author { get; set; } = null!;

	public string Email { get; set; } = null!;

	public string PasswordHash { get; set; } = null!;

	public Role Role { get; set; } = Role.Unspecified;

	public virtual ICollection<Token> Tokens { get; set; } = [];
}
