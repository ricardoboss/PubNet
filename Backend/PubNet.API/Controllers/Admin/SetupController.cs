using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.DTO.Admin;
using PubNet.API.DTO.Authentication;
using PubNet.API.DTO.Errors;
using PubNet.API.Exceptions;
using PubNet.Auth.Models;

namespace PubNet.API.Controllers.Admin;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
public class SetupController(ISetupService setupService, ISiteConfigurationProvider siteConfigurationProvider, IAccountService accountService) : PubNetControllerBase
{
	[HttpGet("Status")]
	public async Task<SetupStatusDto> GetStatus(CancellationToken cancellationToken = default)
	{
		var siteConfiguration = await siteConfigurationProvider.GetAsync(cancellationToken);

		return new()
		{
			RegistrationsOpen = siteConfiguration.RegistrationsOpen,
			SetupComplete = await setupService.IsSetupCompleteAsync(cancellationToken),
		};
	}

	[HttpPost("RootAdmin")]
	[ProducesResponseType<AccountCreatedDto>(StatusCodes.Status201Created)]
	[ProducesResponseType<ValidationErrorsDto>(StatusCodes.Status400BadRequest)]
	public async Task<AccountCreatedDto> CreateAccountAsync(CreateAccountDto dto,
		CancellationToken cancellationToken = default)
	{
		if (await setupService.IsSetupCompleteAsync(cancellationToken))
			throw new SetupAlreadyCompleteException();

		var identity = await accountService.CreateAccountAsync(dto, Role.Admin, cancellationToken);

		var accountCreatedDto = AccountCreatedDto.MapFrom(identity);

		Response.StatusCode = StatusCodes.Status201Created;
		return accountCreatedDto;
	}
}
