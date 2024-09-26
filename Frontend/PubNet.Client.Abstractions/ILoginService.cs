using PubNet.Client.ApiClient.Generated.Models;

namespace PubNet.Client.Abstractions;

public interface ILoginService
{
	Task<LoginSuccessResult> LoginAsync(CreateLoginTokenDto request, CancellationToken cancellationToken = default);
}
