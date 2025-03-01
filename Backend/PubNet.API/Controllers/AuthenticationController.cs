using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.Admin;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Abstractions.Guard;
using PubNet.API.Attributes;
using PubNet.API.DTO.Authentication;
using PubNet.API.DTO.Authors;
using PubNet.API.DTO.Errors;
using PubNet.API.Exceptions.Authentication;
using PubNet.API.Services.Extensions;
using PubNet.Auth;
using PubNet.Auth.Models;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Controllers;

[Route("[controller]")]
[Tags("Authentication")]
[Authorize]
public class AuthenticationController(
	IAccessTokenService accessTokenService,
	IAccountService accountService,
	IAuthProvider authProvider,
	IJwtFactory jwtFactory,
	ISiteConfigurationProvider siteConfigurationProvider,
	IGuard guard) : PubNetControllerBase
{
	[HttpPost("LoginToken")]
	[AllowAnonymous]
	[ProducesResponseType<TokenCreatedDto>(StatusCodes.Status201Created)]
	[ProducesResponseType<AuthErrorDto>(StatusCodes.Status401Unauthorized)]
	public async Task<TokenCreatedDto> CreateLoginTokenAsync(CreateLoginTokenDto dto,
		CancellationToken cancellationToken = default)
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
	[RequireScope(Scopes.PersonalAccessTokens.Create)]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType<ValidationErrorsDto>(StatusCodes.Status400BadRequest)]
	public async Task<TokenCreatedDto> CreatePersonalAccessTokenAsync(CreatePersonalAccessTokenDto dto,
		CancellationToken cancellationToken = default)
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
	[RequireScope(Scopes.PersonalAccessTokens.Create)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public IEnumerable<string> AllowedScopes() => accessTokenService.AllowedScopes;

	[HttpGet("PersonalAccessToken")]
	[RequireAnyScope(Scopes.PersonalAccessTokens.Read)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<TokenCollectionDto> GetPersonalAccessTokenAsync([FromQuery] bool includeExpired = false,
		CancellationToken cancellationToken = default)
	{
		var currentToken = await authProvider.GetCurrentTokenAsync(cancellationToken);

		IEnumerable<Token> tokens = currentToken.Identity.Tokens;
		if (!includeExpired)
			tokens = tokens.Where(t => !t.IsExpired);

		return TokenCollectionDto.MapFrom(tokens, currentToken.Id);
	}

	[HttpDelete("PersonalAccessToken")]
	[RequireScope(Scopes.PersonalAccessTokens.Delete)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType<NotFoundErrorDto>(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeletePersonalAccessTokenAsync([FromQuery] string tokenId,
		CancellationToken cancellationToken = default)
	{
		var tokenUuid = Guid.Parse(tokenId);
		var token = await accessTokenService.GetTokenAsync(tokenUuid, cancellationToken);
		if (token is null)
			return NotFoundDto("token-not-found", "Token id not found");

		var identity = await authProvider.GetCurrentIdentityAsync(cancellationToken);
		if (token.Identity.Id != identity.Id)
			guard.ThrowIf(User).DoesntHaveRole(Role.Admin);

		Debug.Assert(token.Identity.Id == identity.Id || User.IsInRole(Role.Admin.ToClaimValue()!));

		await accessTokenService.DeleteTokenAsync(token, cancellationToken);

		return NoContent();
	}

	[HttpPost("Account")]
	[AllowAnonymous]
	[ProducesResponseType<AccountCreatedDto>(StatusCodes.Status201Created)]
	[ProducesResponseType<GenericErrorDto>(StatusCodes.Status409Conflict)]
	public async Task<IActionResult> CreateAccountAsync(CreateAccountDto dto,
		CancellationToken cancellationToken = default)
	{
		var siteConfiguration = await siteConfigurationProvider.GetAsync(cancellationToken);
		if (!siteConfiguration.RegistrationsOpen)
			throw new RegistrationsClosedException();

		if (dto.Role is not Role.Default)
			return BadRequest(new ValidationErrorsDto
			{
				Title = "Invalid role",
				Errors = new()
				{
					{ "role", ["The role must be 'default'."] },
				},
			});

		var identity = await accountService.CreateAccountAsync(dto, cancellationToken);

		var accountCreatedDto = AccountCreatedDto.MapFrom(identity);

		return StatusCode(StatusCodes.Status201Created, accountCreatedDto);
	}

	[HttpGet("Self")]
	[ProducesResponseType<AuthorDto>(StatusCodes.Status200OK)]
	public async Task<AuthorDto> GetSelfAsync(CancellationToken cancellationToken = default)
	{
		var identity = await authProvider.GetCurrentIdentityAsync(cancellationToken);

		return AuthorDto.MapFrom(identity.Author);
	}
}
