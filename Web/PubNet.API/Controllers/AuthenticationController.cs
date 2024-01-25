using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Attributes;
using PubNet.API.DTO.Authentication;
using PubNet.API.Services.Extensions;
using PubNet.Web;

namespace PubNet.API.Controllers;

[Route("[controller]")]
[Tags("Authentication")]
public class AuthenticationController(IAccessTokenService accessTokenService, IAccountService accountService, IAuthProvider authProvider) : ControllerBase
{
	[HttpPost("LoginToken")]
	[ProducesResponseType<TokenCreatedDto>(StatusCodes.Status201Created)]
	public async Task<TokenCreatedDto> CreateLoginToken(CreateLoginTokenDto dto, CancellationToken cancellationToken = default)
	{
		var jwt = await accessTokenService.CreateLoginTokenAsync(dto, cancellationToken);

		Response.StatusCode = StatusCodes.Status201Created;
		return TokenCreatedDto.MapFrom(jwt);
	}

	[HttpPost("PersonalAccessToken")]
	[Authorize, Guard(Scopes.PersonalAccessTokens.Create)]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<TokenCreatedDto> CreatePersonalAccessToken(CreatePersonalAccessTokenDto dto, CancellationToken cancellationToken = default)
	{
		var identity = await authProvider.GetCurrentIdentityAsync(cancellationToken);

		var jwt = await accessTokenService.CreatePersonalAccessTokenAsync(identity, dto, cancellationToken);

		Response.StatusCode = StatusCodes.Status201Created;
		return TokenCreatedDto.MapFrom(jwt);
	}

	[HttpPost("Account")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<AccountCreatedDto> CreateAccount(CreateAccountDto dto, CancellationToken cancellationToken = default)
	{
		var identity = await accountService.CreateAccountAsync(dto, cancellationToken);

		var accountCreatedDto = AccountCreatedDto.MapFrom(identity);

		Response.StatusCode = StatusCodes.Status201Created;
		return accountCreatedDto;
	}
}
