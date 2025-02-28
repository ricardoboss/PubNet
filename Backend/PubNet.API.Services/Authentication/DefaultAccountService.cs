using PubNet.API.Abstractions.Authentication;
using PubNet.API.Abstractions.CQRS.Commands;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.API.DTO.Authentication;
using PubNet.Auth.Models;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Services.Authentication;

public class DefaultAccountService(IAuthorDao authorDao, IIdentityDao identityDao, IAuthorDmo authorDmo, IIdentityDmo identityDmo) : IAccountService
{
	public async Task<Identity> CreateAccountAsync(CreateAccountDto dto, Role role, CancellationToken cancellationToken = default)
	{
		var existingAuthor = await authorDao.TryFindByUsernameAsync(dto.UserName, cancellationToken);
		if (existingAuthor is not null)
			throw new UserNameAlreadyExistsException(dto.UserName);

		var existingIdentity = await identityDao.TryFindByEmailAsync(dto.Email, cancellationToken);
		if (existingIdentity is not null)
			throw new EmailAlreadyExistsException(dto.Email);

		var author = await authorDmo.CreateAuthorAsync(dto.UserName, cancellationToken);
		var identity = await identityDmo.CreateIdentityAsync(author, dto.Email, dto.Password, role, cancellationToken);

		return identity;
	}
}
