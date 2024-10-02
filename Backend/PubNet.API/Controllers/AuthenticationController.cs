using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Attributes;
using PubNet.API.DTO;
using PubNet.API.DTO.Authentication;
using PubNet.API.DTO.Authors;
using PubNet.API.Exceptions.Authentication;
using PubNet.API.Services.Extensions;
using PubNet.Auth;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Controllers;

[Route("[controller]")]
[Tags("Authentication")]
public class AuthenticationController(IAccessTokenService accessTokenService, IAccountService accountService, IAuthProvider authProvider, IJwtFactory jwtFactory, IConfiguration configuration) : ControllerBase
{
	[HttpPost("LoginToken")]
	[ProducesResponseType<TokenCreatedDto>(StatusCodes.Status201Created)]
	[ProducesResponseType<GenericErrorDto>(StatusCodes.Status401Unauthorized)]
	public async Task<TokenCreatedDto> CreateLoginTokenAsync(CreateLoginTokenDto dto, CancellationToken cancellationToken = default)
	{
		var token = await accessTokenService.CreateLoginTokenAsync(dto, cancellationToken);
		var jwt = jwtFactory.Create(token);

		Response.StatusCode = StatusCodes.Status201Created;
		return new()
		{
			Value = jwt.Value,
			Token = TokenDto.MapFrom(token),
		};
	}

	[HttpPost("PersonalAccessToken")]
	[Authorize, RequireScope(Scopes.PersonalAccessTokens.Create)]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType<ValidationErrorsDto>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<GenericErrorDto>(StatusCodes.Status401Unauthorized)]
	public async Task<TokenCreatedDto> CreatePersonalAccessTokenAsync(CreatePersonalAccessTokenDto dto, CancellationToken cancellationToken = default)
	{
		var identity = await authProvider.GetCurrentIdentityAsync(cancellationToken);

		var token = await accessTokenService.CreatePersonalAccessTokenAsync(identity, dto, cancellationToken);
		var jwt = jwtFactory.Create(token);

		Response.StatusCode = StatusCodes.Status201Created;
		return new()
		{
			Value = jwt.Value,
			Token = TokenDto.MapFrom(token),
		};
	}

	[HttpGet("PersonalAccessToken")]
	[Authorize, RequireAnyScope(Scopes.PersonalAccessTokens.Read, Scopes.PersonalAccessTokens.Create)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<TokenCollectionDto> GetPersonalAccessTokenAsync([FromQuery] bool includeExpired = false, CancellationToken cancellationToken = default)
	{
		var identity = await authProvider.GetCurrentIdentityAsync(cancellationToken);

		IEnumerable<Token> tokens = identity.Tokens;
		if (!includeExpired)
			tokens = tokens.Where(t => !t.IsExpired);

		return TokenCollectionDto.MapFrom(tokens);
	}

	[HttpPost("Account")]
	[ProducesResponseType<AccountCreatedDto>(StatusCodes.Status201Created)]
	[ProducesResponseType<GenericErrorDto>(StatusCodes.Status409Conflict)]
	[ProducesResponseType<ValidationErrorsDto>(StatusCodes.Status400BadRequest)]
	public async Task<AccountCreatedDto> CreateAccountAsync(CreateAccountDto dto, CancellationToken cancellationToken = default)
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
