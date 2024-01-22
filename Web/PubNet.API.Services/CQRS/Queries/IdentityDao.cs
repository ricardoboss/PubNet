using PubNet.API.Abstractions.CQRS.Exceptions;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.Database.Context;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Services.CQRS.Queries;

public class IdentityDao(PubNetContext context) : IIdentityDao
{
	public async Task<Identity> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var identity = await context.Identities.FindAsync([id], cancellationToken);
		if (identity is null)
			throw new IdentityNotFoundException(id);

		return identity;
	}
}
