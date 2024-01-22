﻿using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.CQRS.Queries;

public interface IIdentityDao
{
	Task<Identity> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
