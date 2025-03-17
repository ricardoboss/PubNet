using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.CQRS.Queries;

public interface IIdentityDao
{
	Task<bool> AnyAsync(CancellationToken cancellationToken = default);

	Task<Identity> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

	Task<Identity?> TryFindByEmailAsync(string email, CancellationToken cancellationToken = default);

	Task<IEnumerable<Identity>> GetAllAsync(CancellationToken cancellationToken = default);
}
