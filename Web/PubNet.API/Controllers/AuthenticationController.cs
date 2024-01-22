using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.DTO.Authentication;

namespace PubNet.API.Controllers;

[Route("[controller]/[action]")]
[Tags("Authentication")]
public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
{
	[HttpPost]
	[ProducesResponseType<TokenCreatedDto>(StatusCodes.Status201Created)]
	public Task<TokenCreatedDto> CreateLoginToken(CreateLoginTokenDto dto, CancellationToken cancellationToken = default)
	{
		return authenticationService.CreateLoginTokenAsync(dto, cancellationToken);
	}

	[Authorize]
	[HttpPost]
	[ProducesResponseType<TokenCreatedDto>(StatusCodes.Status201Created)]
	public Task<TokenCreatedDto> CreatePersonalAccessToken(CreatePersonalAccessTokenDto dto, CancellationToken cancellationToken = default)
	{
		return authenticationService.CreatePersonalAccessTokenAsync(dto, cancellationToken);
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> CreateAccount(CreateAccountDto dto, CancellationToken cancellationToken = default)
	{
		await authenticationService.CreateAccountAsync(dto, cancellationToken);

		return NoContent();
	}
}
