using PubNet.Database.Entities;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.Authentication;

public interface IAuthProvider
{
	Task<Author?> TryGetCurrentAuthorAsync(CancellationToken cancellationToken = default);

	Task<Identity?> TryGetCurrentIdentityAsync(CancellationToken cancellationToken = default);

	Task<Token?> TryGetCurrentTokenAsync(CancellationToken cancellationToken = default);
}
