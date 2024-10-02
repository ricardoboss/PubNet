using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Abstractions.Guard;
using PubNet.API.Attributes;
using PubNet.API.DTO;
using PubNet.API.DTO.Authentication;
using PubNet.API.DTO.Authors;
using PubNet.API.Exceptions.Authentication;
using PubNet.API.Services.Extensions;
using PubNet.Auth;
using PubNet.Auth.Models;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Controllers;

[Route("[controller]")]
[Tags("Authentication")]
public class AuthenticationController(IAccessTokenService accessTokenService, IAccountService accountService, IAuthProvider authProvider, IJwtFactory jwtFactory, IRegistrationsService registrationsService, IGuard guard) : ControllerBase
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

	[HttpGet("PersonalAccessToken/AllowedScopes")]
	[Authorize, RequireScope(Scopes.PersonalAccessTokens.Create)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType<GenericErrorDto>(StatusCodes.Status401Unauthorized)]
	public IEnumerable<string> AllowedScopes() => accessTokenService.AllowedScopes;

	[HttpGet("PersonalAccessToken")]
	[Authorize, RequireAnyScope(Scopes.PersonalAccessTokens.Read)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<TokenCollectionDto> GetPersonalAccessTokenAsync([FromQuery] bool includeExpired = false, CancellationToken cancellationToken = default)
	{
		var currentToken = await authProvider.GetCurrentTokenAsync(cancellationToken);

		IEnumerable<Token> tokens = currentToken.Identity.Tokens;
		if (!includeExpired)
			tokens = tokens.Where(t => !t.IsExpired);

		return TokenCollectionDto.MapFrom(tokens, currentToken.Id);
	}

	[HttpDelete("PersonalAccessToken")]
	[Authorize, RequireScope(Scopes.PersonalAccessTokens.Delete)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task DeletePersonalAccessTokenAsync([FromQuery] string tokenId, CancellationToken cancellationToken = default)
	{
		var tokenUuid = Guid.Parse(tokenId);
		var token = await accessTokenService.GetTokenAsync(tokenUuid, cancellationToken);
		if (token is null)
		{
			Response.StatusCode = StatusCodes.Status404NotFound;

			return;
		}

		var identity = await authProvider.GetCurrentIdentityAsync(cancellationToken);
		if (token.Identity.Id != identity.Id)
		{
			guard.ThrowIf(User).HasntRole(Role.Admin);

			Response.StatusCode = StatusCodes.Status403Forbidden;

			return;
		}

		await accessTokenService.DeleteTokenAsync(token, cancellationToken);

		Response.StatusCode = StatusCodes.Status204NoContent;
	}

	[HttpPost("Account")]
	[ProducesResponseType<AccountCreatedDto>(StatusCodes.Status201Created)]
	[ProducesResponseType<GenericErrorDto>(StatusCodes.Status409Conflict)]
	[ProducesResponseType<ValidationErrorsDto>(StatusCodes.Status400BadRequest)]
	public async Task<AccountCreatedDto> CreateAccountAsync(CreateAccountDto dto, CancellationToken cancellationToken = default)
	{
		if (!await AreRegistrationsOpen())
			throw new RegistrationsClosedException();

		var identity = await accountService.CreateAccountAsync(dto, cancellationToken);

		var accountCreatedDto = AccountCreatedDto.MapFrom(identity);

		Response.StatusCode = StatusCodes.Status201Created;
		return accountCreatedDto;
	}

	[HttpGet("RegistrationsOpen")]
	public async Task<bool> AreRegistrationsOpen()
	{
		return await registrationsService.AreRegistrationsOpenAsync();
	}

	[HttpGet("Self")]
	[ProducesResponseType<AuthorDto>(StatusCodes.Status200OK)]
	[ProducesResponseType<GenericErrorDto>(StatusCodes.Status401Unauthorized)]
	public async Task<AuthorDto> GetSelfAsync(CancellationToken cancellationToken = default)
	{
		var identity = await authProvider.GetCurrentIdentityAsync(cancellationToken);

		return AuthorDto.MapFrom(identity.Author);
	}
}
