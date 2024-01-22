using PubNet.API.Abstractions.Authentication;
using PubNet.API.DTO.Authentication;

namespace PubNet.API.Services.Authentication;

public class AccountService : IAccountService
{
	public async Task CreateAccountAsync(CreateAccountDto dto, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
