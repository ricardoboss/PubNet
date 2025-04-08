using Microsoft.EntityFrameworkCore;
using PubNet.API.Abstractions.CQRS.Exceptions;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.Database.Context;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Services.CQRS.Queries;

public class IdentityDao(PubNet2Context context) : IIdentityDao
{
	public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
	{
		return await context.Identities.AnyAsync(cancellationToken);
	}

	public async Task<Identity> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var identity = await context.Identities.FindAsync([id], cancellationToken);
		if (identity is null)
			throw new IdentityNotFoundException(id);

		return identity;
	}

	public Task<Identity?> TryFindByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		return context.Identities.SingleOrDefaultAsync(x => x.Email == email, cancellationToken);
	}
}
