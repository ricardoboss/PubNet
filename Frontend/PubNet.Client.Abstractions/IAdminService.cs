using PubNet.Client.ApiClient.Generated.Models;

namespace PubNet.Client.Abstractions;

public interface IAdminService
{
	Task<IdentityDto> CreateAccountAsync(CreateAccountDto data, CancellationToken cancellationToken = default);

	Task<IEnumerable<IdentityDto>> GetAccountsAsync(CancellationToken cancellationToken = default);

	Task DeleteIdentityAsync(Guid identityId, CancellationToken cancellationToken = default);
}
