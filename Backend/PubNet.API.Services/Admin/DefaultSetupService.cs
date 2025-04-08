﻿using PubNet.API.Abstractions.Admin;
using PubNet.API.Abstractions.CQRS.Queries;

namespace PubNet.API.Services.Admin;

public class DefaultSetupService(IIdentityDao identityDao) : ISetupService
{
	public async Task<bool> IsSetupCompleteAsync(CancellationToken cancellationToken = default)
	{
		var anyUserRegistered = await IsAnyUserRegisteredAsync(cancellationToken);
		if (!anyUserRegistered)
			return false;

		// TODO: add more checks here

		return true;
	}

	private async Task<bool> IsAnyUserRegisteredAsync(CancellationToken cancellationToken = default)
	{
		return await identityDao.AnyAsync(cancellationToken);
	}
}
