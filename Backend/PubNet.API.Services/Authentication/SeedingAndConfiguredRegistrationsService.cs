using Microsoft.Extensions.Configuration;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Abstractions.CQRS.Queries;

namespace PubNet.API.Services.Authentication;

public class SeedingAndConfiguredRegistrationsService(IConfiguration configuration, IIdentityDao identityDao) : IRegistrationsService
{
	public async Task<bool> AreRegistrationsOpenAsync(CancellationToken cancellationToken = default)
	{
		if (configuration.GetValue<bool>("RegistrationsOpen"))
			return true;

		var anyUserExists = await identityDao.AnyAsync(cancellationToken);

		// if no users exist, registrations are open so the admin can create their own account
		return !anyUserExists;
	}
}
