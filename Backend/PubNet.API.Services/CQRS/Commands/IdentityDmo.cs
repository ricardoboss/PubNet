using Microsoft.AspNetCore.Identity;
using PubNet.API.Abstractions.CQRS.Commands;
using PubNet.Database.Context;
using PubNet.Database.Entities;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Services.CQRS.Commands;

public class IdentityDmo(PubNetContext context, IPasswordHasher<Identity> passwordHasher) : IIdentityDmo
{
	public async Task<Identity> CreateIdentityAsync(Author author, string email, string password, CancellationToken cancellationToken = default)
	{
		// TODO: validate email
		// TODO: validate password

		var identity = new Identity
		{
			Author = author,
			Email = email,
		};

		identity.PasswordHash = passwordHasher.HashPassword(identity, password);

		await context.Identities.AddAsync(identity, cancellationToken);

		await context.SaveChangesAsync(cancellationToken);

		await context.Entry(identity).ReloadAsync(cancellationToken);

		return identity;
	}
}
