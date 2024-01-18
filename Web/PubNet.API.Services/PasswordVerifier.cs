using Microsoft.AspNetCore.Identity;
using PubNet.Database.Context;
using PubNet.Database.Entities.Auth;
using PubNet.Web.Abstractions;

namespace PubNet.API.Services;

public class PasswordVerifier(PubNetContext dbContext, IPasswordHasher<Identity> passwordHasher) : IPasswordVerifier
{
	public async Task<bool> VerifyAsync(Guid identityId, string? password, CancellationToken cancellationToken = default)
	{
		if (password is null)
			return false;

		var identity = await dbContext.Identities.FindAsync([identityId], cancellationToken);
		if (identity is null || string.IsNullOrWhiteSpace(identity.PasswordHash))
			return false;

		var result = passwordHasher.VerifyHashedPassword(identity, identity.PasswordHash, password);

		return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
	}
}
