using Microsoft.AspNetCore.Identity;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.Database.Context;
using PubNet.Database.Entities.Auth;
using PubNet.Web.Abstractions;

namespace PubNet.API.Services;

public class PasswordVerifier(IPasswordHasher<Identity> passwordHasher, IIdentityDao identityDao) : IPasswordVerifier
{
	public async Task<bool> VerifyAsync(Guid identityId, string? password, CancellationToken cancellationToken = default)
	{
		if (password is null)
			return false;

		var identity = await identityDao.FindByIdAsync(identityId, cancellationToken);
		if (string.IsNullOrWhiteSpace(identity.PasswordHash))
			return false;

		var result = passwordHasher.VerifyHashedPassword(identity, identity.PasswordHash, password);

		return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
	}
}
