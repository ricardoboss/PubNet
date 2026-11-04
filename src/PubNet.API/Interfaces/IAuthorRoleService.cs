using PubNet.Database.Models;

namespace PubNet.API.Interfaces;

public interface IAuthorRoleService
{
	/// <summary>
	/// Whether the given author is the only remaining admin.
	/// </summary>
	Task<bool> IsLastAdminAsync(Author author, CancellationToken cancellationToken = default);

	/// <summary>
	/// Changes an author's role.
	/// </summary>
	/// <exception cref="Services.LastAdminException">The author is the last admin and would lose the role.</exception>
	Task SetRoleAsync(Author author, Role role, CancellationToken cancellationToken = default);
}
