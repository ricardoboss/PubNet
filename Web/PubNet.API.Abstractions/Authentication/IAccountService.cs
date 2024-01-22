using PubNet.API.DTO.Authentication;

namespace PubNet.API.Abstractions.Authentication;

public interface IAccountService
{
	Task CreateAccountAsync(CreateAccountDto dto, CancellationToken cancellationToken = default);
}
