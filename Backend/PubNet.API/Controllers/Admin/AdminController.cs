using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Attributes;
using PubNet.API.DTO.Admin;
using PubNet.API.DTO.Authentication;
using PubNet.API.DTO.Errors;
using PubNet.Auth.Models;

namespace PubNet.API.Controllers.Admin;

[ApiController]
[Route("[controller]")]
[Authorize, RequireRole(Role.Admin)]
public class AdminController(IAccountService accountService) : PubNetControllerBase
{
	[HttpPost("Account")]
	[ProducesResponseType<IdentityDto>(StatusCodes.Status201Created)]
	[ProducesResponseType<GenericErrorDto>(StatusCodes.Status409Conflict)]
	public async Task<IdentityDto> CreateAccountAsync(CreateAccountDto data,
		CancellationToken cancellationToken = default)
	{
		var identity = await accountService.CreateAccountAsync(data, cancellationToken);

		return IdentityDto.MapFrom(identity);
	}
}
