using PubNet.API.DTO.Authentication;
using PubNet.Auth.Models;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.Authentication;

public interface IAccountService
{
	/// <exception cref="UserNameAlreadyExistsException">Thrown when the user name already exists.</exception>
	Task<Identity> CreateAccountAsync(CreateAccountDto dto, CancellationToken cancellationToken = default);
}
