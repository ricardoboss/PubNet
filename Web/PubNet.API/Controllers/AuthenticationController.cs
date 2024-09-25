using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Attributes;
using PubNet.API.DTO;
using PubNet.API.DTO.Authentication;
using PubNet.API.DTO.Authors;
using PubNet.API.Exceptions.Authentication;
using PubNet.API.Services.Extensions;
using PubNet.Database.Entities.Auth;
using PubNet.Web;

namespace PubNet.API.Controllers;

[Route("[controller]")]
[Tags("Authentication")]
public class AuthenticationController(IAccessTokenService accessTokenService, IAccountService accountService, IAuthProvider authProvider, IConfiguration configuration) : ControllerBase
{
	[HttpPost("LoginToken")]
	[ProducesResponseType<TokenCreatedDto>(StatusCodes.Status201Created)]
	[ProducesResponseType<GenericErrorDto>(StatusCodes.Status401Unauthorized)]
	public async Task<TokenCreatedDto> CreateLoginToken(CreateLoginTokenDto dto, CancellationToken cancellationToken = default)
	{
		var jwt = await accessTokenService.CreateLoginTokenAsync(dto, cancellationToken);

		Response.StatusCode = StatusCodes.Status201Created;
		return TokenCreatedDto.MapFrom(jwt);
	}

	[HttpPost("PersonalAccessToken")]
	[Authorize, RequireScope(Scopes.PersonalAccessTokens.Create)]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<TokenCreatedDto> CreatePersonalAccessToken(CreatePersonalAccessTokenDto dto, CancellationToken cancellationToken = default)
	{
		var identity = await authProvider.GetCurrentIdentityAsync(cancellationToken);

		var jwt = await accessTokenService.CreatePersonalAccessTokenAsync(identity, dto, cancellationToken);

		Response.StatusCode = StatusCodes.Status201Created;
		return TokenCreatedDto.MapFrom(jwt);
	}

	[HttpGet("PersonalAccessToken")]
	[Authorize, RequireAnyScope(Scopes.PersonalAccessTokens.Read, Scopes.PersonalAccessTokens.Create)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<TokenCollectionDto> GetPersonalAccessToken([FromQuery] bool includeExpired = false, CancellationToken cancellationToken = default)
	{
		var identity = await authProvider.GetCurrentIdentityAsync(cancellationToken);

		IEnumerable<Token> tokens = identity.Tokens;
		if (!includeExpired)
			tokens = tokens.Where(t => !t.IsExpired);

		return TokenCollectionDto.MapFrom(tokens);
	}

	[HttpPost("Account")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<AccountCreatedDto> CreateAccount(CreateAccountDto dto, CancellationToken cancellationToken = default)
	{
		if (!AreRegistrationsOpen())
			throw new RegistrationsClosedException();

		var identity = await accountService.CreateAccountAsync(dto, cancellationToken);

		var accountCreatedDto = AccountCreatedDto.MapFrom(identity);

		Response.StatusCode = StatusCodes.Status201Created;
		return accountCreatedDto;
	}

	[HttpGet("RegistrationsOpen")]
	public bool AreRegistrationsOpen()
	{
		return configuration.GetValue<bool>("RegistrationsOpen");
	}

	[HttpGet("Self")]
	[ProducesResponseType<AuthorDto>(StatusCodes.Status200OK)]
	public async Task<AuthorDto> GetSelfAsync(CancellationToken cancellationToken = default)
	{
		var identity = await authProvider.GetCurrentIdentityAsync(cancellationToken);

		return AuthorDto.MapFrom(identity.Author);
	}
}
