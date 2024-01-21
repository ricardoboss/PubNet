using Microsoft.AspNetCore.Mvc;
using PubNet.API.DTO.Authentication;

namespace PubNet.API.Controllers;

[Route("[controller]/[action]")]
[Tags("Authentication")]
public class AuthenticationController : ControllerBase
{
	[HttpPost]
	[ProducesResponseType<TokenCreatedDto>(StatusCodes.Status201Created)]
	public Task<TokenCreatedDto> CreateToken(CreateTokenDto dto, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public Task<IActionResult> CreateAccount(CreateAccountDto dto, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
