using PubNet.Client.Generated.Models;
using PubNet.Web.Models;

namespace PubNet.Client.Abstractions;

public interface ILoginService
{
	Task<LoginSuccessResult> LoginAsync(CreateLoginTokenDto request, CancellationToken cancellationToken = default);
}

public record LoginSuccessResult(JsonWebToken Token);
