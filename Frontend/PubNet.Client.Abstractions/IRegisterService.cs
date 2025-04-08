using PubNet.Client.ApiClient.Generated.Models;

namespace PubNet.Client.Abstractions;

public interface IRegisterService
{
	Task<bool> AreRegistrationsOpenAsync(CancellationToken cancellationToken = default);

	/// <exception cref="RegisterException">In case the setup is already complete.</exception>
	Task RegisterRootAdminAsync(CreateAccountDto request, CancellationToken cancellationToken = default);

	/// <exception cref="RegisterException">In case username or email are already taken.</exception>
	Task RegisterAsync(CreateAccountDto request, CancellationToken cancellationToken = default);
}
