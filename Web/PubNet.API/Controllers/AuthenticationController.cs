using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.DTO.Authentication;
using PubNet.API.Services.Extensions;

namespace PubNet.API.Controllers;

[Route("[controller]/[action]")]
[Tags("Authentication")]
public class AuthenticationController(IAccessTokenService accessTokenService, IAccountService accountService, IAuthProvider authProvider) : ControllerBase
{
	private string IpAddress => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

	private string UserAgent => Request.Headers.UserAgent.ToString();

	[HttpPost]
	[ProducesResponseType<TokenCreatedDto>(StatusCodes.Status201Created)]
	public Task<TokenCreatedDto> CreateLoginToken(CreateLoginTokenDto dto, CancellationToken cancellationToken = default)
	{
		return accessTokenService.CreateLoginTokenAsync(dto, IpAddress, UserAgent, cancellationToken);
	}

	[Authorize]
	[HttpPost]
	[ProducesResponseType<TokenCreatedDto>(StatusCodes.Status201Created)]
	public async Task<TokenCreatedDto> CreatePersonalAccessToken(CreatePersonalAccessTokenDto dto, CancellationToken cancellationToken = default)
	{
		var identity = await authProvider.GetCurrentIdentityAsync(cancellationToken);

		return await accessTokenService.CreatePersonalAccessTokenAsync(identity, dto, IpAddress, UserAgent, cancellationToken);
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> CreateAccount(CreateAccountDto dto, CancellationToken cancellationToken = default)
	{
		await accountService.CreateAccountAsync(dto, cancellationToken);

		return NoContent();
	}
}
