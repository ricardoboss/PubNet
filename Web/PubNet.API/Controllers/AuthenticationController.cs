using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Attributes;
using PubNet.API.DTO.Authentication;
using PubNet.API.Services.Extensions;
using PubNet.Web.Abstractions;

namespace PubNet.API.Controllers;

[Route("[controller]/[action]")]
[Tags("Authentication")]
public class AuthenticationController(IAccessTokenService accessTokenService, IAccountService accountService, IAuthProvider authProvider) : ControllerBase
{
	[HttpPost]
	[ProducesResponseType<TokenCreatedDto>(StatusCodes.Status201Created)]
	public Task<TokenCreatedDto> CreateLoginToken(CreateLoginTokenDto dto, CancellationToken cancellationToken = default)
	{
		return accessTokenService.CreateLoginTokenAsync(dto, cancellationToken);
	}

	[HttpPost]
	[Authorize]
	[ScopeGuard(Scopes.PersonalAccessTokens.Create)]
	[ProducesResponseType<TokenCreatedDto>(StatusCodes.Status201Created)]
	public async Task<TokenCreatedDto> CreatePersonalAccessToken(CreatePersonalAccessTokenDto dto, CancellationToken cancellationToken = default)
	{
		var identity = await authProvider.GetCurrentIdentityAsync(cancellationToken);

		return await accessTokenService.CreatePersonalAccessTokenAsync(identity, dto, cancellationToken);
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> CreateAccount(CreateAccountDto dto, CancellationToken cancellationToken = default)
	{
		await accountService.CreateAccountAsync(dto, cancellationToken);

		return NoContent();
	}
}
